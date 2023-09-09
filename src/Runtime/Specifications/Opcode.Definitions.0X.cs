using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler;
using Chips.Compiler.Utility;
using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using Chips.Utility;
using Chips.Utility.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodeNop : BasicOpcode {
		public override OpcodeID Code => OpcodeID.Nop;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Instructions.Add(CilOpCodes.Nop);
			return true;
		}
	}

	public sealed class OpcodeBrk : BasicOpcode {
		public override OpcodeID Code => OpcodeID.Brk;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Instructions.Add(CilOpCodes.Break);
			return true;
		}
	}

	public sealed class OpcodeLdci : LoadConstantOpcode {
		public override string Register => nameof(Registers.A);

		public override OpcodeID Code => OpcodeID.Ldci;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (!ValidateArgumentAndEmitNumberRegisterAccess(context, args, out IInteger? argAsInteger, "an integer"))
				return false;

			var instructions = context.Instructions;

			// Push the constant and convert it when necessary
			// Then call the appropriate method in IntegerRegister
			switch (ValueConverter.UpcastToAtLeastInt32(argAsInteger)) {
				case Int32_T i:
					instructions.Add(CilOpCodes.Ldc_I4, i.ActualValue);
					Compile_Int32Load(context, argAsInteger);
					break;
				case UInt32_T u:
					instructions.Add(CilOpCodes.Ldc_I4, unchecked((int)u.ActualValue));
					instructions.Add(CilOpCodes.Conv_U4);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(uint))!));
					break;
				case Int64_T li:
					instructions.Add(CilOpCodes.Ldc_I8, li.ActualValue);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(long))!));
					break;
				case UInt64_T lu:
					instructions.Add(CilOpCodes.Ldc_I8, unchecked((long)lu.ActualValue));
					instructions.Add(CilOpCodes.Conv_U8);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(ulong))!));
					break;
			}

			return true;
		}

		private static void Compile_Int32Load(CompilationContext context, IInteger argAsInteger) {
			var instructions = context.Instructions;

			switch (argAsInteger) {
				case SByte_T:
					instructions.Add(CilOpCodes.Conv_I1);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(sbyte))!));
					break;
				case Byte_T:
					instructions.Add(CilOpCodes.Conv_U1);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(byte))!));
					break;
				case Int16_T:
					instructions.Add(CilOpCodes.Conv_I2);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(short))!));
					break;
				case UInt16_T:
					instructions.Add(CilOpCodes.Conv_U2);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(ushort))!));
					break;
				default:
					// Int32_T
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(IntegerRegister).GetCachedMethod("Set", typeof(int))!));
					break;
			}
		}

		protected override object? DeserializeArgument(BinaryReader reader) {
			byte type = reader.ReadByte();
			
			return type switch {
				0 => reader.ReadInt32(),
				1 => reader.ReadUInt32(),
				2 => reader.ReadInt64(),
				3 => reader.ReadUInt64(),
				_ => throw new InvalidDataException($"Invalid integer type {type}"),
			};
		}

		protected override object? ParseArgument(CompilationContext context, string arg) {
			return ParseIntegerArgument(context, arg);
		}

		protected override void SerializeArgument(BinaryWriter writer, object? arg) {
			// Argument is guaranteed to be an IInteger by this point, but it needs to be casted again
			switch (ValueConverter.UpcastToAtLeastInt32(ValueConverter.BoxToUnderlyingType(arg)!)) {
				case Int32_T i:
					writer.Write((byte)0);
					writer.Write(i.ActualValue);
					break;
				case UInt32_T u:
					writer.Write((byte)1);
					writer.Write(u.ActualValue);
					break;
				case Int64_T l:
					writer.Write((byte)2);
					writer.Write(l.ActualValue);
					break;
				case UInt64_T ul:
					writer.Write((byte)3);
					writer.Write(ul.ActualValue);
					break;
			}
		}
	}

	public sealed class OpcodeLdcf : LoadConstantOpcode {
		public override string Register => nameof(Registers.I);

		public override OpcodeID Code => OpcodeID.Ldcf;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (!ValidateArgumentAndEmitNumberRegisterAccess(context, args, out IFloat? argAsFloat, "a floating-point"))
				return false;

			var instructions = context.Instructions;

			// Push the constant and convert it when necessary
			// Then call the appropriate method in FloatRegister
			switch (argAsFloat) {
				case Single_T s:
					instructions.Add(CilOpCodes.Ldc_R4, s.ActualValue);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(FloatRegister).GetCachedMethod("Set", typeof(float))!));
					break;
				case Double_T d:
					instructions.Add(CilOpCodes.Ldc_R8, d.ActualValue);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(FloatRegister).GetCachedMethod("Set", typeof(double))!));
					break;
				case Decimal_T m:
					instructions.LoadDecimalConstant(m.ActualValue, context.importer);
					instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(FloatRegister).GetCachedMethod("Set", typeof(decimal))!));
					break;
			}

			return true;
		}

		protected override object? DeserializeArgument(BinaryReader reader) {
			byte type = reader.ReadByte();
			
			return type switch {
				0 => reader.ReadSingle(),
				1 => reader.ReadDouble(),
				2 => reader.ReadDecimal(),
				_ => throw new InvalidDataException($"Invalid floating-point type {type}"),
			};
		}

		protected override object? ParseArgument(CompilationContext context, string arg) {
			return ParseFloatArgument(context, arg);
		}

		protected override void SerializeArgument(BinaryWriter writer, object? arg) {
			// Argument is guaranteed to be an IFloat by this point, but it needs to be casted again
			switch (ValueConverter.BoxToUnderlyingType(arg)) {
				case Single_T s:
					writer.Write((byte)0);
					writer.Write(s.ActualValue);
					break;
				case Double_T d:
					writer.Write((byte)1);
					writer.Write(d.ActualValue);
					break;
				case Decimal_T m:
					writer.Write((byte)2);
					writer.Write(m.ActualValue);
					break;
			}
		}
	}

	public sealed class OpcodeLdcs : LoadConstantOpcode {
		public override string Register => nameof(Registers.S);

		public override OpcodeID Code => OpcodeID.Ldcs;

		// Ldcs is the odd one out of the "load constant" opcodes, so it needs a specialized method
		private bool ValidateArgumentAndEmitStringRegisterAcces(CompilationContext context, OpcodeArgumentCollection args, out StringMetadata arg) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			if (args[0] is not StringMetadata argAsType)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects a string metadata token argument"));

			context.Instructions.Add(CilOpCodes.Ldsfld, context.importer.ImportField(typeof(Registers).GetCachedField(Register)!));

			arg = argAsType;
			return true;
		}

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (!ValidateArgumentAndEmitStringRegisterAcces(context, args, out StringMetadata argAsToken))
				return false;

			var instructions = context.Instructions;

			instructions.Add(CilOpCodes.Ldstr, context.heap.GetString(argAsToken));
			instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(StringRegister).GetCachedProperty("Value")!.SetMethod!));

			return true;
		}

		protected override object? DeserializeArgument(BinaryReader reader) {
			return StringMetadata.Deserialize(reader);
		}

		protected override object? ParseArgument(CompilationContext context, string arg) {
			// Convert escape sequences to the actual characters
			return context.heap.GetOrAdd(arg.SanitizeString());
		}

		protected override void SerializeArgument(BinaryWriter writer, object? arg) {
			// Argument is guaranteed to be a string metadata token for the string heap by this point, but it needs to be casted again
			((StringMetadata)arg!).Serialize(writer);
		}
	}

	public sealed class OpcodeLdfi : LoadFieldOpcode {
		public override bool LoadsAddress => false;
		
		public override bool LoadsStaticField => false;

		public override OpcodeID Code => OpcodeID.Ldfi;
	}

	public sealed class OpcodeLdfs : LoadFieldOpcode {
		public override bool LoadsAddress => false;

		public override bool LoadsStaticField => true;

		public override OpcodeID Code => OpcodeID.Ldfs;
	}
}
