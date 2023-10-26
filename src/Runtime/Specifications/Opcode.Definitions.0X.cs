using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler;
using Chips.Compiler.Utility;
using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Utility;
using System;
using System.IO;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodeNop : BasicOpcode {
		public override OpcodeID Code => OpcodeID.Nop;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Nop);
		}
	}

	public sealed class OpcodeBrk : BasicOpcode {
		public override OpcodeID Code => OpcodeID.Brk;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Break);
		}
	}

	public sealed class OpcodeLdci : LoadConstantOpcode<int> {
		public override string Register => nameof(Registers.A);

		public override OpcodeID Code => OpcodeID.Ldci;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context, args, out int value);
			
			// Push the constant
			context.Cursor.Emit(CilOpCodes.Ldc_I4, value);
			context.EmitNumberRegisterAssignment<IntegerRegister, int>();
		}

		protected override int DeserializeArgument(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			return reader.ReadInt32();
		}

		protected override int ParseArgument(CompilationContext context, string arg) {
			return StringSerialization.ParseIntegerArgument(context, arg) is not int i
				? throw ChipsCompiler.ErrorAndThrow(new ParsingException("Argument was too large for a 32-bit signed constant"))
				: i;
		}

		protected override void SerializeArgument(BinaryWriter writer, int arg, TypeResolver resolver, StringHeap heap) {
			writer.Write(arg);
		}
	}

	public sealed class OpcodeLdcf : LoadConstantOpcode<float> {
		public override string Register => nameof(Registers.I);

		public override OpcodeID Code => OpcodeID.Ldcf;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context, args, out float value);

			// Push the constant, then call the appropriate method in FloatRegister
			context.Cursor.Emit(CilOpCodes.Ldc_R4, value);
			context.EmitNumberRegisterAssignment<FloatRegister, float>();
		}

		protected override float DeserializeArgument(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			return reader.ReadSingle();
		}

		protected override float ParseArgument(CompilationContext context, string arg) {
			return StringSerialization.ParseFloatArgument(context, arg) is not float f
				? throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Constant {arg} was too large to be a single-precisio floating point number"))
				: f;
		}

		protected override void SerializeArgument(BinaryWriter writer, float arg, TypeResolver resolver, StringHeap heap) {
			writer.Write(arg);
		}
	}

	public sealed class OpcodeLdcs : LoadConstantOpcode<StringMetadata> {
		public override string Register => nameof(Registers.S);

		public override OpcodeID Code => OpcodeID.Ldcs;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context, args, out StringMetadata arg);

			context.Cursor.Emit(CilOpCodes.Ldstr, context.heap.GetString(arg));
			context.EmitRegisterValueAssignment<StringRegister>();
		}

		protected override StringMetadata DeserializeArgument(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			return StringMetadata.Deserialize(reader);
		}

		protected override StringMetadata ParseArgument(CompilationContext context, string arg) {
			if (arg is null)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Use \"ldz.s\" to load null into the &S register"));

			// Convert escape sequences to the actual characters
			return context.heap.GetOrAdd(arg.SanitizeString());
		}

		protected override void SerializeArgument(BinaryWriter writer, StringMetadata arg, TypeResolver resolver, StringHeap heap) {
			arg.Serialize(writer);
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

	public sealed class OpcodeLdrg : LoadMethodVariableOpcode {
		public override OpcodeID Code => OpcodeID.Ldrg;

		public override bool LoadsLocal => false;

		public override bool LoadsAddress => false;
	}

	public sealed class OpcodeLdlc : LoadMethodVariableOpcode {
		public override OpcodeID Code => OpcodeID.Ldlc;

		public override bool LoadsLocal => true;

		public override bool LoadsAddress => false;
	}

	// ldmtd

	public sealed class OpcodeLdzs : LoadZeroOpcode {
		public override string Register => nameof(Registers.S);

		public override OpcodeID Code => OpcodeID.Ldzs;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context);

			context.Cursor.Emit(CilOpCodes.Ldnull);
			context.EmitRegisterValueAssignment<StringRegister>();
		}
	}

	public sealed class OpcodeLdelX : LoadElementInArrayOpcode {
		public override bool LoadsAddress => false;

		public override bool IndexWithXRegister => true;

		public override OpcodeID Code => OpcodeID.Ldel_X;
	}

	public sealed class OpcodeComp : Opcode {
		public override OpcodeID Code => OpcodeID.Comp;

		public override int ExpectedArgumentCount => 0;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*    C# code:
			 *    
			 *    __compare = ValueConverter.CheckedBoxToUnderlyingType((object)value2);
			 *    ValueConverter.CheckedBoxToUnderlyingType((object)value1).Compare(__compare);
			 */

			int local = context.CreateOrGetLocal<INumber>("__compare");

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			context.EmitNopAndDelayedResolver(static (body, index) => new DelayedBoxResolver(body, index));

			context.EmitBoxToUnderlyingType();
			context.Cursor.Emit(CilOpCodes.Stloc, local);

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			context.EmitNopAndDelayedResolver(static (body, index) => new DelayedBoxResolver(body, index));

			context.EmitBoxToUnderlyingType();

			context.Cursor.Emit(CilOpCodes.Ldloc, local);

			context.EmitFunctionCall<INumber>(nameof(INumber.Compare));
		}

		public override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}

	public sealed class OpcodeIs : TypeOperandOpcode {
		public override OpcodeID Code => OpcodeID.Is;

		public override bool AllowsNull => true;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (args[0] is null) {
				// Check against null directly
				context.EmitRegisterLoad("F");
				context.Cursor.Emit(CilOpCodes.Ldnull);
				context.Cursor.Emit(CilOpCodes.Ceq);
				context.EmitFlagAssignment(nameof(FlagsRegister.Conversion));
				return;
			}

			var resolver = (DelayedTypeResolver)args[0]!;
			
			/*    C# code:
			 *    
			 *    Registers.F.Conversion = value is ArgType;
			 */

			context.EmitRegisterLoad("F");

			// Local capturing
			var r = resolver;
			context.EmitNopAndDelayedResolver((body, index) => new DelayedIsinstResolver(body, index, r));

			context.EmitFlagAssignment(nameof(FlagsRegister.Conversion));
		}
	}

	// conv

	public sealed class OpcodeConv : TypeOperandOpcode {
		public override OpcodeID Code => OpcodeID.Conv;

		public override bool AllowsNull => false;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			var resolver = (DelayedTypeResolver)args[0]!;

			if (resolver.metadata == "object") {
				// Box or castclass
				context.EmitNopAndDelayedResolver(static (body, index) => new DelayedBoxOrImplicitObjectResolver(body, index));
			}
		}
	}

	public sealed class OpcodeKbrdy : Opcode {
		public override OpcodeID Code => OpcodeID.Kbrdy;
		
		public override int ExpectedArgumentCount => 0;

		public override unsafe nint Method => (nint)(delegate*<bool>)&Implementation.Kbrdy;

		public override void GetMethodSignature(out Type returnType, out Type[] parameterTypes) {
			returnType = typeof(bool);
			parameterTypes = Type.EmptyTypes;
		}

		public override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}
}
