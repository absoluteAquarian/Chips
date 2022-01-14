using Chips.Core.Meta;
using Chips.Core.Specifications;
using Chips.Core.Types;
using Chips.Utility;
using Chips.Utility.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Numerics;

namespace Chips.Compilation{
	internal abstract class OpcodeArgument{
		public readonly OperandInformationType type;

		private readonly byte[] data;

		public ReadOnlySpan<byte> Data => new(data);

		public OpcodeArgument(byte[] data, OperandInformationType type){
			this.data = data;
			this.type = type;
		}

		internal abstract void Compile(ILProcessor il, string sourceFile);
	}

	internal class OpcodeArgumentConstant : OpcodeArgument{
		public readonly OperandConstantType valueType;

		public ReadOnlySpan<byte> ValueData => Data[1..];

		public OpcodeArgumentConstant(byte[] data) : base(data, OperandInformationType.Constant){
			valueType = (OperandConstantType)data[0];

			switch(valueType){
				case OperandConstantType.BigInt:
				case OperandConstantType.Object:
				case OperandConstantType.Array:
				case OperandConstantType.List:
				case OperandConstantType.Time:
				case OperandConstantType.Set:
				case OperandConstantType.Date:
				case OperandConstantType.Regex:
				case OperandConstantType.Random:
				case OperandConstantType.UserDef:
					throw new IOException($"Constant type {valueType} cannot be used as a constant");
			}
		}

		internal override void Compile(ILProcessor il, string sourceFile){
			switch(valueType){
				case OperandConstantType.I32:
				case OperandConstantType.I16:
				case OperandConstantType.I8:
				case OperandConstantType.U32:
				case OperandConstantType.U8:
				case OperandConstantType.U16:
				case OperandConstantType.Char:
				case OperandConstantType.Bool:
					byte[] arr = new byte[4];
					ValueData.CopyTo(arr);
					il.Emit(OpCodes.Ldc_I4, BitConverter.ToInt32(arr));
					break;
				case OperandConstantType.I64:
					il.Emit(OpCodes.Ldc_I8, BitConverter.ToInt64(ValueData));
					break;
				case OperandConstantType.U64:
					il.Emit(OpCodes.Ldc_I8, BitConverter.ToInt64(ValueData));
					break;
				case OperandConstantType.F32:
				case OperandConstantType.F16:
					il.Emit(OpCodes.Ldc_R4, BitConverter.ToSingle(ValueData));

					if(valueType == OperandConstantType.F16)
						il.Emit(OpCodes.Call, il.Body.Method.DeclaringType.Module.ImportReference(typeof(Half).GetCachedExplicitOperator(typeof(float))));
					break;
				case OperandConstantType.F64:
					il.Emit(OpCodes.Ldc_R8, BitConverter.ToDouble(ValueData));
					break;
				case OperandConstantType.F128:
					ReadOnlySpan<int> span = ValueData.ConvertSpan(4);  //Get four ints
					il.PushDecimalConstant(sourceFile, span);
					break;
				case OperandConstantType.String:
					int index = 0;
					string str = ValueData.ToArray().GetStringFromData(ref index);
					il.Emit(OpCodes.Ldstr, str);
					break;
				default:
					Compiler.exceptions.Add(new(sourceFile, "Could not compile constant type: " + valueType));
					break;
			}
		}
	}

	//Handles Local/Global variables, Registers and Flags
	internal class OpcodeArgumentVariable : OpcodeArgument{
		public readonly bool IsLocalVariable;
		public readonly bool IsGlobalVariable;
		public readonly bool IsRegister;
		public readonly bool IsFlag;

		public readonly int variableIndex;
		public readonly int? spRegisterOffset;

		private readonly int valueDataOffset;

		public ReadOnlySpan<byte> VariableData => Data[valueDataOffset..];

		public OpcodeArgumentVariable(byte[] data) : this(data, OperandInformationType.Variable){ }

		internal protected OpcodeArgumentVariable(byte[] data, OperandInformationType typeOverride) : base(data, typeOverride){
			byte flags = data[0];
			int index = data.Get7BitEncodedInt(1, out int offset);
			valueDataOffset = offset + 1;

			byte mask = (byte)(flags & 0b11);
			if(mask == 0)
				IsLocalVariable = true;
			else if(mask == 1)
				IsGlobalVariable = true;
			else if(mask == 2)
				IsRegister = true;
			else if(mask == 3)
				IsFlag = true;

			variableIndex = index;

			//&SP register
			//Bit 2 set = stack access, instead of SP index access
			if(IsRegister && variableIndex == 4 && (flags & 0b0000_0100) != 0)
				spRegisterOffset = data.Get7BitEncodedInt(valueDataOffset, out valueDataOffset);
		}

		internal override void Compile(ILProcessor il, string sourceFile){
			var module = il.Body.Method.DeclaringType.Module;

			if(IsRegister){
				string? register = variableIndex switch{
					0 => "A",
					1 => "E",
					2 => "X",
					3 => "Y",
					4 => "SP",
					5 => "S",
					_ => null
				};

				if(register is null){
					Compiler.exceptions.Add(new(sourceFile, "Invalid register ID: " + variableIndex));
					return;
				}

				FieldReference field = module.ImportReference(typeof(Metadata.Registers).GetCachedField(register));

				bool isMov = BytecodeFunction.CompilingOpcode.descriptor.StartsWith("mov ");

				//The register should only be treated as the instance itself if the instruction is "mov"
				il.Emit(OpCodes.Ldsfld, field);
				if(!isMov)
					il.Emit(OpCodes.Call, module.ImportReference(typeof(Register).GetCachedProperty("Data")!.GetGetMethod()));
			}else if(IsFlag){
				string? flag = variableIndex switch{
					0 => "Carry",
					1 => "Conversion",
					2 => "Comparison",
					3 => "PropertyAccess",
					4 => "RegexSuccess",
					5 => "Zero",
					_ => null
				};

				if(flag is null){
					Compiler.exceptions.Add(new(sourceFile, "Invalid flag ID: " + variableIndex));
					return;
				}

				MethodReference property = module.ImportReference(typeof(Metadata.Flags).GetCachedProperty(flag)!.GetGetMethod());

				il.Emit(OpCodes.Call, property);
			}

			// TODO: local/global variables
		}
	}

	internal class OpcodeArgumentLabel : OpcodeArgument{
		public readonly int labelIndex;

		public OpcodeArgumentLabel(byte[] data) : base(data, OperandInformationType.Label){
			labelIndex = data.Get7BitEncodedInt(0, out _);
		}
	}

	//Used in "debug" builds
	internal class OpcodeArgumentTypeString : OpcodeArgument{
		public readonly string referencedType;

		public OpcodeArgumentTypeString(byte[] data) : base(data, OperandInformationType.TypeString){
			int index = 0;
			referencedType = data.GetStringFromData(ref index);
		}
	}

	//Used in "release" builds
	internal class OpcodeArgumentTypeCode : OpcodeArgument{
		private readonly Type? referencedType;

		public readonly string? userDefinedType = null;

		public Type? ReferencedType => referencedType ?? (Metadata.userDefinedTypes.TryGetValue(userDefinedType!, out var userType) ? userType : null);

		public OpcodeArgumentTypeCode(byte[] data) : this(data, OperandInformationType.TypeCode){ }

		public OpcodeArgumentTypeCode(byte[] data, OperandInformationType typeOverride) : base(data, typeOverride){
			OperandConstantType varType = (OperandConstantType)data[0];

			referencedType = varType switch{
				OperandConstantType.I32 => typeof(int),
				OperandConstantType.I8 => typeof(sbyte),
				OperandConstantType.I16 => typeof(short),
				OperandConstantType.I64 => typeof(long),
				OperandConstantType.U32 => typeof(uint),
				OperandConstantType.U8 => typeof(byte),
				OperandConstantType.U16 => typeof(ushort),
				OperandConstantType.U64 => typeof(ulong),
				OperandConstantType.BigInt => typeof(BigInteger),
				OperandConstantType.F32 => typeof(float),
				OperandConstantType.F64 => typeof(double),
				OperandConstantType.F128 => typeof(decimal),
				OperandConstantType.Object => typeof(object),
				OperandConstantType.Char => typeof(char),
				OperandConstantType.String => typeof(string),
				OperandConstantType.Indexer => typeof(Core.Types.Indexer),
				OperandConstantType.Array => typeof(Array),
				OperandConstantType.Range => typeof(Core.Types.Range),
				OperandConstantType.List => typeof(Core.Types.List),
				OperandConstantType.Time => typeof(TimeSpan),
				OperandConstantType.Set => typeof(Core.Types.ArithmeticSet),
				OperandConstantType.Date => typeof(DateTime),
				OperandConstantType.Regex => typeof(Core.Types.Regex),
				OperandConstantType.Bool => typeof(bool),
				OperandConstantType.Random => typeof(Random),
				OperandConstantType.Complex => typeof(Complex),
				OperandConstantType.UserDef => null,
				OperandConstantType.F16 => typeof(Half),
				_ => throw new Exception("Unknown constant type code: " + varType)
			};

			if(referencedType == typeof(Array)){
				byte dimensions = data[1];

				for(int i = 0; i < dimensions; i++)
					referencedType = referencedType.MakeArrayType();
			}else if(referencedType is null){
				int index = 1;
				userDefinedType = data.GetStringFromData(ref index);
			}
		}
	}

	internal class OpcodeArgumentFunctionCall : OpcodeArgument{
		public readonly string functionIdentifier;

		public OpcodeArgumentFunctionCall(byte[] data) : base(data, OperandInformationType.FunctionCall){
			int index = 0;
			functionIdentifier = data.GetStringFromData(ref index);
		}
	}

	/// <summary>
	/// Maps to <seealso cref="Opcode.GetValueIndirectly"/> or <seealso cref="Opcode.SetValueIndirectly"/>
	/// </summary>
	internal class OpcodeArgumentCollectionAccessIndexByX : OpcodeArgumentVariable{
		public OpcodeArgumentCollectionAccessIndexByX(byte[] data) : base(data, OperandInformationType.CollectionAccessIndexByX){ }
	}

	/// <summary>
	/// Maps to <seealso cref="Opcode.GetValueIndirectly"/> or <seealso cref="Opcode.SetValueIndirectly"/>
	/// </summary>
	internal class OpcodeArgumentCollectionAccessIndexByY : OpcodeArgumentVariable{
		public OpcodeArgumentCollectionAccessIndexByY(byte[] data) : base(data, OperandInformationType.CollectionAccessIndexByY){ }
	}
}
