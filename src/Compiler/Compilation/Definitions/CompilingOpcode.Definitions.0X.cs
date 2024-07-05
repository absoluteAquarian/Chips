using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler.Utility;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;
using Chips.Utility;
using System.IO;
using System;
using AsmResolver.DotNet;
using Chips.Runtime;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;

namespace Chips.Compiler.Compilation {
	public sealed class CompilingOpcodeNop : BasicCompilingOpcode<OpcodeNop> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Nop);
		}
	}

	public sealed class CompilingOpcodeBrk : BasicCompilingOpcode<OpcodeBrk> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Break);
		}
	}

	public sealed class CompilingOpcodeLdci : LoadConstantCompilingOpcode<OpcodeLdci, Int32_T> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context, args, out Int32_T value);
			
			// Push the constant
			context.EmitNumber(value);
			context.EmitNumberRegisterAssignment<IntegerRegister, int>();
		}

		public override Int32_T DeserializeArgument(CompilationContext context, BinaryReader reader) {
			return reader.ReadInt32().CastToInt32_T();
		}

		public override Int32_T ParseArgument(CompilationContext context, string arg) {
			return StringSerialization.ParseIntegerArgument(context, arg) is not int i
				? throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Constant {arg} was not a 32-bit signed constant"))
				: i.CastToInt32_T();
		}

		public override void SerializeArgument(BinaryWriter writer, Int32_T arg, CompilationContext context) {
			writer.Write(arg.ActualValue);
		}
	}

	public sealed class CompilingOpcodeLdcf : LoadConstantCompilingOpcode<OpcodeLdcf, Single_T> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context, args, out Single_T value);

			// Push the constant, then call the appropriate method in FloatRegister
			context.EmitNumber(value);
			context.EmitNumberRegisterAssignment<FloatRegister, float>();
		}

		public override Single_T DeserializeArgument(CompilationContext context, BinaryReader reader) {
			return reader.ReadSingle().CastToSingle_T();
		}

		public override Single_T ParseArgument(CompilationContext context, string arg) {
			return StringSerialization.ParseFloatArgument(context, arg) is not float f
				? throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Constant {arg} was not a single-precision floating point number"))
				: f.CastToSingle_T();
		}

		public override void SerializeArgument(BinaryWriter writer, Single_T arg, CompilationContext context) {
			writer.Write(arg.ActualValue);
		}
	}

	public sealed class CompilingOpcodeLdcs : LoadConstantCompilingOpcode<OpcodeLdcs, StringMetadata> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context, args, out StringMetadata arg);

			context.Cursor.Emit(CilOpCodes.Ldstr, context.heap.GetString(arg));
			context.EmitRegisterValueAssignment<StringRegister>();
		}

		public override StringMetadata DeserializeArgument(CompilationContext context, BinaryReader reader) {
			return StringMetadata.Deserialize(reader);
		}

		public override StringMetadata ParseArgument(CompilationContext context, string arg) {
			if (arg is null)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Use \"ldz.s\" to load null into the &S register"));

			// Convert escape sequences to the actual characters
			return context.heap.GetOrAdd(arg.SanitizeString());
		}

		public override void SerializeArgument(BinaryWriter writer, StringMetadata arg, CompilationContext context) {
			arg.Serialize(writer);
		}
	}

	public sealed class CompilingOpcodeLdfi : LoadFieldCompilingOpcode<OpcodeLdfi> { }

	public sealed class CompilingOpcodeLdfs : LoadFieldCompilingOpcode<OpcodeLdfs> { }

	public sealed class CompilingOpcodeLdrg : LoadMethodVariableCompilingOpcode<OpcodeLdrg> { }

	public sealed class CompilingOpcodeLdlc : LoadMethodVariableCompilingOpcode<OpcodeLdlc> { }

	// ldmtd

	public sealed class CompilingOpcodeLdzs : LoadZeroCompilingOpcode<OpcodeLdzs> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context);

			context.Cursor.Emit(CilOpCodes.Ldnull);
			context.EmitRegisterValueAssignment<StringRegister>();
		}
	}

	public sealed class CompilingOpcodeLdelX : LoadElementInArrayCompilingOpcode<OpcodeLdelX> { }

	public sealed class CompilingOpcodeComp : CompilingOpcode<OpcodeComp> {
		public override int ExpectedArgumentCount => 0;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*    C# code:
			 *    
			 *    __compare = (object)VALUE;
			 *    Implementation.Compare((object)VALUE2, __compare);
			 */

			int local = context.CreateOrGetLocal<object>("__compare");
			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			context.EmitNopAndDelayedResolver<DelayedBoxOrImplicitObjectResolver>();
			context.Cursor.Emit(CilOpCodes.Stloc, local);

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			context.EmitNopAndDelayedResolver<DelayedBoxOrImplicitObjectResolver>();
			context.Cursor.Emit(CilOpCodes.Ldloc, local);
			context.EmitImplementationCall(nameof(Implementation.Compare));
		}

		public override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) => null;

		public override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) { }
	}

	public sealed class CompilingOpcodeIs : TypeOperandCompilingOpcode<OpcodeIs> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			int local = context.CreateOrGetLocal<object>("__is");
			context.EmitNopAndDelayedResolver<DelayedBoxOrImplicitObjectResolver>();
			context.Cursor.Emit(CilOpCodes.Stloc, local);

			if (args[0] is null) {
				// Check against null directly
				/*    C# code:
				 *    
				 *    object __is = (object)VALUE;
				 *    Registers.F.Conversion = __is is null;
				 */

				context.EmitRegisterLoad(nameof(Registers.F));
				context.Cursor.Emit(CilOpCodes.Ldloc, local);
				context.Cursor.Emit(CilOpCodes.Ldnull);
				context.Cursor.Emit(CilOpCodes.Ceq);
				context.EmitFlagAssignment(nameof(FlagsRegister.Conversion));
				return;
			}

			var resolver = args.GetValue<DelayedTypeResolver>(0);
			
			/*    C# code:
			 *    
			 *    object __is = (object)VALUE;
			 *    Registers.F.Conversion = __is is ArgType;
			 */

			context.EmitRegisterLoad(nameof(Registers.F));
			context.Cursor.Emit(CilOpCodes.Ldloc, local);
			context.EmitNopAndDelayedResolver<DelayedIsinstResolver, ITypeDefOrRef>(resolver);
			context.EmitFlagAssignment(nameof(FlagsRegister.Conversion));
		}
	}

	public sealed class CompilingOpcodeConv : TypeOperandCompilingOpcode<OpcodeConv> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			var resolver = args.GetValue<DelayedTypeResolver>(0);

			if (resolver.metadata == "object") {
				// Box or castclass
				context.EmitNopAndDelayedResolver<DelayedBoxOrCastclassObjectResolver>();
			}
		}
	}

	public sealed class CompilingOpcodeKbrdy : BasicCompilingOpcode<OpcodeKbrdy> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			throw this.ThrowNotImplemented();
		}
	}
}
