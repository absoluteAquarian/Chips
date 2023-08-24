using Chips.Runtime.Meta;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;
using Chips.Utility;
using Chips.Utility.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Chips.Compilation {
	internal abstract class OpcodeArgument {
		public readonly OperandInformationType type;

		private readonly byte[] data;

		public ReadOnlySpan<byte> Data => new(data);

		public OpcodeArgument(byte[] data, OperandInformationType type) {
			this.data = data;
			this.type = type;
		}

		internal abstract void Compile(ILProcessor il, string sourceFile);

		internal virtual void Bake(BinaryWriter writer, string sourceFile) {
			writer.Write(Data);
		}
	}

	internal class OpcodeArgumentConstant : OpcodeArgument {
		public readonly OperandType valueType;

		public ReadOnlySpan<byte> ValueData => Data[1..];

		public OpcodeArgumentConstant(byte[] data) : base(data, OperandInformationType.Constant) {
			valueType = (OperandType)data[0];

			switch (valueType) {
				case OperandType.BigInt:
				case OperandType.Object:
				case OperandType.Array:
				case OperandType.List:
				case OperandType.Time:
				case OperandType.Set:
				case OperandType.Date:
				case OperandType.Regex:
				case OperandType.Random:
				case OperandType.UserDef:
					throw new IOException($"Constant type {valueType} cannot be used as a constant");
			}
		}

		internal override void Bake(BinaryWriter writer, string sourceFile) {
			switch (valueType) {
				case OperandType.I32:
				case OperandType.I16:
				case OperandType.I8:
				case OperandType.U32:
				case OperandType.U8:
				case OperandType.U16:
				case OperandType.Char:
				case OperandType.Bool:
				case OperandType.I64:
				case OperandType.U64:
				case OperandType.F32:
				case OperandType.F16:
				case OperandType.F64:
				case OperandType.F128:
				case OperandType.String:
					writer.Write(Data);
					break;
				default:
					ChipsCompiler.exceptions.Add(new(sourceFile, "Could not compile constant type: " + valueType));
					break;
			}
		}

		internal override void Compile(ILProcessor il, string sourceFile) {
			switch (valueType) {
				case OperandType.I32:
				case OperandType.I16:
				case OperandType.I8:
				case OperandType.U32:
				case OperandType.U8:
				case OperandType.U16:
				case OperandType.Char:
				case OperandType.Bool:
					byte[] arr = new byte[4];
					ValueData.CopyTo(arr);
					il.Emit(OpCodes.Ldc_I4, BitConverter.ToInt32(arr));
					break;
				case OperandType.I64:
					il.Emit(OpCodes.Ldc_I8, BitConverter.ToInt64(ValueData));
					break;
				case OperandType.U64:
					il.Emit(OpCodes.Ldc_I8, BitConverter.ToInt64(ValueData));
					break;
				case OperandType.F32:
				case OperandType.F16:
					il.Emit(OpCodes.Ldc_R4, BitConverter.ToSingle(ValueData));

					if (valueType == OperandType.F16)
						il.Emit(OpCodes.Call, il.Body.Method.DeclaringType.Module.ImportReference(typeof(Half).GetCachedExplicitOperator(typeof(float))));
					break;
				case OperandType.F64:
					il.Emit(OpCodes.Ldc_R8, BitConverter.ToDouble(ValueData));
					break;
				case OperandType.F128:
					ReadOnlySpan<int> span = ValueData.ConvertSpan(4);  //Get four ints
					il.PushDecimalConstant(sourceFile, span);
					break;
				case OperandType.String:
					int index = 0;
					string str = ValueData.ToArray().GetStringFromData(ref index);
					il.Emit(OpCodes.Ldstr, str);
					break;
				default:
					ChipsCompiler.exceptions.Add(new(sourceFile, "Could not compile constant type: " + valueType));
					break;
			}
		}
	}

	//Handles Local/Global variables, Registers and Flags
	internal class OpcodeArgumentVariable : OpcodeArgument {
		public readonly bool IsLocalVariable;
		public readonly bool IsGlobalVariable;
		public readonly bool IsRegister;
		public readonly bool IsFlag;

		public readonly int variableIndex;
		public readonly int? spRegisterOffset;

		private readonly int valueDataOffset;

		public ReadOnlySpan<byte> VariableData => Data[valueDataOffset..];

		public OpcodeArgumentVariable(byte[] data) : this(data, OperandInformationType.Variable) { }

		internal protected OpcodeArgumentVariable(byte[] data, OperandInformationType typeOverride) : base(data, typeOverride) {
			byte flags = data[0];

			int index = data.Get7BitEncodedInt(1, out int offset);
			valueDataOffset = offset + 1;

			byte mask = (byte)(flags & 0b11);
			if (mask == 0)
				IsLocalVariable = true;
			else if (mask == 1)
				IsGlobalVariable = true;
			else if (mask == 2)
				IsRegister = true;
			else if (mask == 3)
				IsFlag = true;

			variableIndex = index;

			//&SP register
			//Bit 2 set = stack access, instead of SP index access
			if (IsRegister && variableIndex == 4 && (flags & 0b0000_0100) != 0)
				spRegisterOffset = data.Get7BitEncodedInt(valueDataOffset, out valueDataOffset);
		}

		internal override void Compile(ILProcessor il, string sourceFile) {
			var module = il.Body.Method.DeclaringType.Module;

			if (IsRegister) {
				string? register = variableIndex switch {
					0 => "A",
					1 => "E",
					2 => "X",
					3 => "Y",
					4 => "SP",
					5 => "S",
					_ => null
				};

				if (register is null) {
					ChipsCompiler.exceptions.Add(new(sourceFile, "Invalid register ID: " + variableIndex));
					return;
				}

				FieldReference field = module.ImportReference(typeof(Metadata.Registers).GetCachedField(register));

				bool isMov = BytecodeFunction.CompilingOpcode.descriptor.StartsWith("mov ");

				//The register should only be treated as the instance itself if the instruction is "mov"
				il.Emit(OpCodes.Ldsfld, field);
				if (!isMov)
					il.Emit(OpCodes.Call, module.ImportReference(typeof(Register).GetCachedProperty("Data")!.GetGetMethod()));
			} else if (IsFlag) {
				string? flag = variableIndex switch {
					0 => "Carry",
					1 => "Conversion",
					2 => "Comparison",
					3 => "PropertyAccess",
					4 => "RegexSuccess",
					5 => "Zero",
					_ => null
				};

				if (flag is null) {
					ChipsCompiler.exceptions.Add(new(sourceFile, "Invalid flag ID: " + variableIndex));
					return;
				}

				MethodReference property = module.ImportReference(typeof(Metadata.Flags).GetCachedProperty(flag)!.GetGetMethod());

				il.Emit(OpCodes.Call, property);
			}

			// TODO: local/global variables
		}
	}

	internal class OpcodeArgumentLabel : OpcodeArgument {
		public readonly int labelIndex;

		public OpcodeArgumentLabel(byte[] data) : base(data, OperandInformationType.Label) {
			labelIndex = data.Get7BitEncodedInt(0, out _);
		}

		internal override void Compile(ILProcessor il, string sourceFile) {
			throw new InvalidOperationException("Label argument cannot be compiled");
		}
	}

	//Used in "debug" builds
	internal class OpcodeArgumentTypeString : OpcodeArgument {
		public readonly string referencedType;

		public OpcodeArgumentTypeString(byte[] data) : base(data, OperandInformationType.TypeString) {
			int index = 0;
			referencedType = data.GetStringFromData(ref index);
		}

		internal override void Bake(BinaryWriter writer, string sourceFile) {
			throw new NotImplementedException();
		}

		internal override void Compile(ILProcessor il, string sourceFile) {
			throw new NotImplementedException();
		}
	}

	//Used in "release" builds
	internal class OpcodeArgumentTypeCode : OpcodeArgument {
		private readonly Type? referencedType;

		public readonly string? userDefinedType = null;

		public Type? ReferencedType => referencedType ?? (Metadata.userDefinedTypes.TryGetValue(userDefinedType!, out var userType) ? userType : null);

		public OpcodeArgumentTypeCode(byte[] data) : this(data, OperandInformationType.TypeCode) { }

		public OpcodeArgumentTypeCode(byte[] data, OperandInformationType typeOverride) : base(data, typeOverride) {
			OperandType varType = (OperandType)data[0];

			referencedType = varType switch {
				OperandType.I32 => typeof(int),
				OperandType.I8 => typeof(sbyte),
				OperandType.I16 => typeof(short),
				OperandType.I64 => typeof(long),
				OperandType.U32 => typeof(uint),
				OperandType.U8 => typeof(byte),
				OperandType.U16 => typeof(ushort),
				OperandType.U64 => typeof(ulong),
				OperandType.BigInt => typeof(BigInteger),
				OperandType.F32 => typeof(float),
				OperandType.F64 => typeof(double),
				OperandType.F128 => typeof(decimal),
				OperandType.Object => typeof(object),
				OperandType.Char => typeof(char),
				OperandType.String => typeof(string),
				OperandType.Indexer => typeof(Runtime.Types.Indexer),
				OperandType.Array => typeof(Array),
				OperandType.Range => typeof(Runtime.Types.Range),
				OperandType.List => typeof(Runtime.Types.List),
				OperandType.Time => typeof(TimeSpan),
				OperandType.Set => typeof(Runtime.Types.ArithmeticSet),
				OperandType.Date => typeof(DateTime),
				OperandType.Regex => typeof(Runtime.Types.Regex),
				OperandType.Bool => typeof(bool),
				OperandType.Random => typeof(Random),
				OperandType.Complex => typeof(Complex),
				OperandType.UserDef => null,
				OperandType.F16 => typeof(Half),
				_ => throw new Exception("Unknown constant type code: " + varType)
			};

			if (referencedType == typeof(Array)) {
				byte dimensions = data[1];

				for (int i = 0; i < dimensions; i++)
					referencedType = referencedType.MakeArrayType();
			} else if (referencedType is null) {
				int index = 1;
				userDefinedType = data.GetStringFromData(ref index);
			}
		}

		internal override void Bake(BinaryWriter writer, string sourceFile) {
			throw new NotImplementedException();
		}

		internal override void Compile(ILProcessor il, string sourceFile) {
			throw new NotImplementedException();
		}
	}

	internal class OpcodeArgumentFunctionCall : OpcodeArgument {
		// Formats:
		//   NAMESPACE.TYPE::METHOD(ARG,LIST)
		//   TYPE::METHOD(ARG,LIST)
		//   TYPE<GENERIC,ARGS>::METHOD(ARG,LIST)
		public readonly string functionIdentifier;

		public OpcodeArgumentFunctionCall(byte[] data) : base(data, OperandInformationType.FunctionCall) {
			int index = 0;
			functionIdentifier = data.GetStringFromData(ref index);
		}

		internal override void Bake(BinaryWriter writer, string sourceFile) {
			throw new NotImplementedException();
		}

		internal override void Compile(ILProcessor il, string sourceFile) {
			// Parse identifier

		}

		private void GetFunctionInformation(string sourceFile, out Type type, out MethodInfo method) {
			type = null!;
			method = null!;

			int len = functionIdentifier.Length;

			StringBuilder phrase = new();

			for (int i = 0; i < len; i++) {
				char c = functionIdentifier[i];

				if (type is null && c == ':') {
					// Next char must also be a colon
					if (c == len - 1 || functionIdentifier[len + 1] != ':') {
						ChipsCompiler.exceptions.Add(new CompilationException(sourceFile, "Parent type for function declaration could not be resolved"));
						return;
					}


				}
			}
		}
	}

	/// <summary>
	/// Maps to <seealso cref="Opcode.GetValueIndirectly"/> or <seealso cref="Opcode.SetValueIndirectly"/>
	/// </summary>
	internal class OpcodeArgumentCollectionAccessIndexByX : OpcodeArgumentVariable {
		public OpcodeArgumentCollectionAccessIndexByX(byte[] data) : base(data, OperandInformationType.CollectionAccessIndexByX) { }
	}

	/// <summary>
	/// Maps to <seealso cref="Opcode.GetValueIndirectly"/> or <seealso cref="Opcode.SetValueIndirectly"/>
	/// </summary>
	internal class OpcodeArgumentCollectionAccessIndexByY : OpcodeArgumentVariable {
		public OpcodeArgumentCollectionAccessIndexByY(byte[] data) : base(data, OperandInformationType.CollectionAccessIndexByY) { }
	}
}
