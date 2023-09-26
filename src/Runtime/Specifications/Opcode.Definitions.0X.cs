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
			if (ValueConverter.UpcastToAtLeastInt32(argAsInteger) is not Int32_T value)
				return false;

			instructions.Add(CilOpCodes.Ldc_I4, value.ActualValue);
			Compile_Int32Load(context, argAsInteger);

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
			return reader.ReadInt32();
		}

		protected override object? ParseArgument(CompilationContext context, string arg) {
			return StringSerialization.ParseIntegerArgument(context, arg);
		}

		protected override void SerializeArgument(BinaryWriter writer, object? arg) {
			// Argument is guaranteed to be an integer by this point, but it needs to be casted again
			if (ValueConverter.UpcastToAtLeastInt32(ValueConverter.BoxToUnderlyingType(arg)!) is not Int32_T value)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Constant {arg} was not a signed 32-bit integer or was a larger integer type"));

			writer.Write(value.ActualValue);
		}
	}

	public sealed class OpcodeLdcf : LoadConstantOpcode {
		public override string Register => nameof(Registers.I);

		public override OpcodeID Code => OpcodeID.Ldcf;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (!ValidateArgumentAndEmitNumberRegisterAccess(context, args, out IFloat? argAsFloat, "a floating-point") || argAsFloat is not Single_T value)
				return false;

			var instructions = context.Instructions;

			// Push the constant, then call the appropriate method in FloatRegister
			instructions.Add(CilOpCodes.Ldc_R4, value.ActualValue);
			instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(FloatRegister).GetCachedMethod("Set", typeof(float))!));

			return true;
		}

		protected override object? DeserializeArgument(BinaryReader reader) {
			return reader.ReadSingle();
		}

		protected override object? ParseArgument(CompilationContext context, string arg) {
			return StringSerialization.ParseFloatArgument(context, arg) is float f
				? f
				: throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Constant {arg} was not a single-precision floating-point number"));
		}

		protected override void SerializeArgument(BinaryWriter writer, object? arg) {
			// Argument is guaranteed to be an IFloat by this point, but it needs to be casted again
			if (ValueConverter.BoxToUnderlyingType(arg) is not Single_T value)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Constant {arg} was not a single-precision floating-point number"));

			writer.Write(value.ActualValue);
		}
	}

	public sealed class OpcodeLdcs : LoadConstantOpcode {
		public override string Register => nameof(Registers.S);

		public override OpcodeID Code => OpcodeID.Ldcs;

		// Ldcs is the odd one out of the "load constant" opcodes, so it needs a specialized method
		private bool ValidateArgumentAndEmitStringRegisterAcces(CompilationContext context, OpcodeArgumentCollection args, out StringMetadata arg) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			if (args[0] is not StringMetadata argAsType)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects a string metadata token argument"));

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

	public sealed class OpcodeLdrg : Opcode {
		public override OpcodeID Code => OpcodeID.Ldrg;

		public override StackBehavior StackBehavior => StackBehavior.PushOne;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			if (args[0] is not ushort arg)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects an unsigned 16-bit integer as the argument"));

			context.Instructions.Add(CilOpCodes.Ldarg, arg);
			return true;
		}

		public override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			return new OpcodeArgumentCollection()
				.Add(reader.ReadUInt16());
		}

		public override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			if (args.Length != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			return new OpcodeArgumentCollection()
				.Add(StringSerialization.ParseSimpleSmallIntegerArgument(context, args[0]));
		}

		public override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			writer.Write((ushort)args[0]!);
		}
	}

	public sealed class OpcodeLdlc : Opcode {
		public override OpcodeID Code => OpcodeID.Ldlc;

		public override StackBehavior StackBehavior => StackBehavior.PushOne;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			if (args[0] is not ushort arg)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects an unsigned 16-bit integer as the argument"));

			context.Instructions.Add(CilOpCodes.Ldloc, arg);
			return true;
		}

		public override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			return new OpcodeArgumentCollection()
				.Add(reader.ReadUInt16());
		}

		public override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			if (args.Length != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			return new OpcodeArgumentCollection()
				.Add(StringSerialization.ParseSimpleSmallIntegerArgument(context, args[0]));
		}

		public override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			writer.Write((ushort)args[0]!);
		}
	}
}
