﻿using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler.Utility;
using Chips.Runtime;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;
using Chips.Runtime.Utility;
using Chips.Utility;
using System;
using System.IO;

namespace Chips.Compiler.Compilation {
	public abstract class CompilingOpcode {
		/// <summary>
		/// The expected number of arguments to be given to this instructino
		/// </summary>
		public abstract int ExpectedArgumentCount { get; }

		/// <summary>
		/// Gets the runtime representation of this opcode
		/// </summary>
		public abstract Opcode GetRuntimeOpcode();

		/// <summary>
		/// If <see cref="Opcode.Method"/> is used, this method is called to put the opcode object on the stack.<br/>
		/// Defaults to calling the default constructor for the object returned by <see cref="GetRuntimeOpcode"/>
		/// </summary>
		/// <param name="context">An object containing information about the current method being compiled</param>
		public virtual void CompileOpcodeLoad(CompilationContext context) {
			var constructor = this.GetOrCreateConstructor();
			context.Cursor.Emit(CilOpCodes.Newobj, context.importer.ImportMethod(constructor));
		}

		/// <summary>
		/// Add the CIL instructions used when transpiling this Chips instruction here
		/// </summary>
		/// <param name="context">An object containing information about the current method being compiled</param>
		/// <param name="args">The arguments used for this instruction</param>
		/// <remarks>If no instructions are added, the default implementation of loading the static opcode field and calling its function is used instead</remarks>
		public abstract void Compile(CompilationContext context, OpcodeArgumentCollection args);

		/// <summary>
		/// Deserialize the argument collection from the data stream here
		/// </summary>
		/// <param name="context">An object containing information about the current method being read</param>
		/// <param name="reader">The data stream</param>
		/// <returns>A collection of arguments.  Return <see langword="null"/> to indicate that this opcode has no arguments</returns>
		public abstract OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader);

		/// <summary>
		/// Parse the arguments from the string representation here
		/// </summary>
		/// <param name="context">An object containing information about the current method being compiled</param>
		/// <param name="args">The list of arguments, which were previously in a comma-separated list</param>
		/// <returns>A collection of arguments.  Return <see langword="null"/> to indicate that this opcode has no arguments</returns>
		public abstract OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args);

		/// <summary>
		/// Serialize the argument collection to the data stream here
		/// </summary>
		/// <param name="context">An object containing information about the current method being written</param>
		/// <param name="writer">The data stream</param>
		/// <param name="args">The collection of arguments</param>
		public abstract void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args);
	}

	/// <summary>
	/// The base classs for a compiling Chips instruction
	/// </summary>
	public abstract class CompilingOpcode<TOpcode> : CompilingOpcode where TOpcode : Opcode, new() {
		public override Opcode GetRuntimeOpcode() => new TOpcode();

		/// <inheritdoc cref="GetRuntimeOpcode"/>
		public TOpcode GetDirectRuntimeOpcode() => (TOpcode)GetRuntimeOpcode();
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> that has no arguments
	/// </summary>
	public abstract class BasicCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : Opcode, new() {
		public sealed override int ExpectedArgumentCount => 0;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) { }
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a constant value to a register
	/// </summary>
	public abstract class LoadConstantCompilingOpcode<TOpcode, T> : CompilingOpcode<TOpcode> where TOpcode : LoadConstantOpcode<T>, new() {
		public sealed override int ExpectedArgumentCount => 1;

		private T ValidateArgument(OpcodeArgumentCollection args) {
			if (args[0] is not T arg)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{GetRuntimeOpcode().Name}\" expects a {typeof(T).GetSimplifiedGenericTypeName()} argument, received \"{args[0]?.GetType().GetSimplifiedGenericTypeName() ?? "null"}\" instead"));

			return arg;
		}

		protected void EmitRegisterAccess(CompilationContext context, OpcodeArgumentCollection args, out T arg) {
			arg = ValidateArgument(args);

			context.EmitRegisterLoad(GetDirectRuntimeOpcode().Register);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) {
			try {
				return new OpcodeArgumentCollection()
					.Add(DeserializeArgument(context, reader));
			} catch (Exception ex) {
				ChipsCompiler.ErrorAndThrow(ex);
				throw;
			}
		}

		protected abstract T DeserializeArgument(CompilationContext context, BinaryReader reader);

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			try {
				return new OpcodeArgumentCollection()
					.Add(ParseArgument(context, args[0]));
			} catch (Exception ex) {
				ChipsCompiler.ErrorAndThrow(ex);
				throw;
			}
		}

		protected abstract T ParseArgument(CompilationContext context, string arg);

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) {
			var registerName = GetDirectRuntimeOpcode().Register;

			Register register = typeof(Registers).RetrieveStaticField<Register>(registerName)!;

			var arg = ValidateArgument(args);
			
			if (!register.AcceptsValue(arg))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Register \"{registerName}\" does not accept values of type \"{arg?.GetType().GetSimplifiedGenericTypeName() ?? "null"}\""));

			try {
				SerializeArgument(writer, arg, context);
			} catch (Exception ex) {
				ChipsCompiler.ErrorAndThrow(ex);
				throw;
			}
		}

		protected abstract void SerializeArgument(BinaryWriter writer, T arg, CompilationContext context);
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a "zero" to a register
	/// </summary>
	public abstract class LoadZeroCompilingOpcode<TOpcode> : BasicCompilingOpcode<TOpcode> where TOpcode : LoadZeroOpcode, new() {
		protected void EmitRegisterAccess(CompilationContext context) {
			context.EmitRegisterLoad(GetDirectRuntimeOpcode().Register);
		}
	}

	public abstract class FieldAccessCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : FieldAccessOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		protected void ValidateFieldArgument(CompilationContext context, OpcodeArgumentCollection args, out IFieldDescriptor importedField, out bool accessesStaticField) {
			var opcode = GetDirectRuntimeOpcode();

			FieldDefinition fieldDefinition;
			if (args[0] is DelayedFieldResolver delayed)
				fieldDefinition = delayed.Member ?? throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" could not evaluate its argument"));
			else if (args[0] is FieldDefinition def)
				fieldDefinition = def;
			else
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" could not evaluate its argument"));

			// Field is static, but opcode expects an instance field or vice versa
			if (fieldDefinition.IsStatic != opcode.AccessesStaticField)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" expects an identifier for {(opcode.AccessesStaticField ? "a static" : "an instance")} field.  Field \"{fieldDefinition.Name}\" in type \"{fieldDefinition.DeclaringType!.Name}\" is {(fieldDefinition.IsStatic ? "a static" : "an instance")} field"));

			// Emit the field access
			importedField = context.importer.ImportField(fieldDefinition);
			accessesStaticField = opcode.AccessesStaticField;
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) {
			var field = reader.ReadFieldDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(field);

			return new OpcodeArgumentCollection()
				.Add(field);
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			return new OpcodeArgumentCollection()
				.Add(new DelayedFieldResolver(context.resolver, args[0]));
		}

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) {
			var arg = args[0];
			if (arg is not IFieldDescriptor field)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException("Argument was not an IFieldDescriptor instance"));

			writer.Write(field, context.heap);
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a field or field address to the stack
	/// </summary>
	public abstract class LoadFieldCompilingOpcode<TOpcode> : FieldAccessCompilingOpcode<TOpcode> where TOpcode : LoadFieldOpcode, new() {
		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			ValidateFieldArgument(context, args, out var importedField, out bool loadsStaticField);

			var opcode = GetDirectRuntimeOpcode();

			if (loadsStaticField)
				context.Cursor.Emit(opcode.LoadsAddress ? CilOpCodes.Ldsflda : CilOpCodes.Ldsfld, importedField);
			else
				context.Cursor.Emit(opcode.LoadsAddress ? CilOpCodes.Ldflda : CilOpCodes.Ldfld, importedField);
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that pops a value from the stack and stores it to a field
	/// </summary>
	public abstract class StoreToFieldCompilingOpcode<TOpcode> : FieldAccessCompilingOpcode<TOpcode> where TOpcode : StoreToFieldOpcode, new() {
		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			ValidateFieldArgument(context, args, out var importedField, out bool storesToStaticField);

			context.Cursor.Emit(storesToStaticField ? CilOpCodes.Stsfld : CilOpCodes.Stfld, importedField);
		}
	}

	public abstract class MethodVariableAccessCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : MethodVariableAccessOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		protected void ValidateArgument(CompilationContext context, OpcodeArgumentCollection args, out ushort arg) {
			var opcode = GetDirectRuntimeOpcode();

			// TODO: referencing method arguments and locals by name instead of index

			if (args[0] is not ushort a)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" expects an unsigned 16-bit integer as the argument"));

			arg = a;
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) {
			return new OpcodeArgumentCollection()
				.Add(reader.ReadUInt16());
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			return new OpcodeArgumentCollection()
				.Add(StringSerialization.ParseSimpleSmallIntegerArgument(context, args[0]));
		}

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) {
			writer.Write(args.GetValue<ushort>(0));
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a method argument or local to the stack
	/// </summary>
	public abstract class LoadMethodVariableCompilingOpcode<TOpcode> : MethodVariableAccessCompilingOpcode<TOpcode> where TOpcode : LoadMethodVariableOpcode, new() {
		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			ValidateArgument(context, args, out ushort arg);

			var opcode = GetDirectRuntimeOpcode();

			if (opcode.LoadsAddress)
				context.Cursor.Emit(opcode.LoadsLocal ? CilOpCodes.Ldloca : CilOpCodes.Ldarga, arg);
			else {
				CilOpCode cilOpCode = opcode.LoadsLocal ? GetLdloc(arg) : GetLdarg(arg);

				switch (cilOpCode.Code) {
					case CilCode.Ldloc_0:
					case CilCode.Ldloc_1:
					case CilCode.Ldloc_2:
					case CilCode.Ldloc_3:
					case CilCode.Ldarg_0:
					case CilCode.Ldarg_1:
					case CilCode.Ldarg_2:
					case CilCode.Ldarg_3:
						context.Cursor.Emit(cilOpCode);
						break;
					default:
						context.Cursor.Emit(cilOpCode, arg);
						break;
				}
			}
		}

		private static CilOpCode GetLdloc(ushort arg) {
			return arg switch {
				0 => CilOpCodes.Ldloc_0,
				1 => CilOpCodes.Ldloc_1,
				2 => CilOpCodes.Ldloc_2,
				3 => CilOpCodes.Ldloc_3,
				_ => arg <= byte.MaxValue ? CilOpCodes.Ldloc_S : CilOpCodes.Ldloc
			};
		}

		private static CilOpCode GetLdarg(ushort arg) {
			return arg switch {
				0 => CilOpCodes.Ldarg_0,
				1 => CilOpCodes.Ldarg_1,
				2 => CilOpCodes.Ldarg_2,
				3 => CilOpCodes.Ldarg_3,
				_ => arg <= byte.MaxValue ? CilOpCodes.Ldarg_S : CilOpCodes.Ldarg
			};
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that pops a value from the stack and stores it to a method argument or local
	/// </summary>
	public abstract class StoreToMethodVariableCompilingOpcode<TOpcode> : MethodVariableAccessCompilingOpcode<TOpcode> where TOpcode : StoreToMethodVariableOpcode, new() {
		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			ValidateArgument(context, args, out ushort arg);

			var opcode = GetDirectRuntimeOpcode();

			context.Cursor.Emit(opcode.StoresToLocal ? GetStloc(arg) : GetStarg(arg), arg);

			CilOpCode cilOpCode = opcode.StoresToLocal ? GetStloc(arg) : GetStarg(arg);

			switch (cilOpCode.Code) {
				case CilCode.Stloc_0:
				case CilCode.Stloc_1:
				case CilCode.Stloc_2:
				case CilCode.Stloc_3:
					context.Cursor.Emit(cilOpCode);
					break;
				default:
					context.Cursor.Emit(cilOpCode, arg);
					break;
			}
		}

		private static CilOpCode GetStloc(ushort arg) {
			return arg switch {
				0 => CilOpCodes.Stloc_0,
				1 => CilOpCodes.Stloc_1,
				2 => CilOpCodes.Stloc_2,
				3 => CilOpCodes.Stloc_3,
				_ => arg <= byte.MaxValue ? CilOpCodes.Stloc_S : CilOpCodes.Stloc
			};
		}

		private static CilOpCode GetStarg(ushort arg) {
			return arg <= byte.MaxValue ? CilOpCodes.Starg_S : CilOpCodes.Starg;
		}
	}

	public abstract class ArrayElementAccessCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : ArrayElementAccessOpcode, new() {
		public sealed override int ExpectedArgumentCount => 0;

		protected void ValidateArgumentAndEmitInstructions(CompilationContext context) {
			var opcode = GetDirectRuntimeOpcode();

			if (opcode.IndexWithXRegister)
				context.EmitRegisterLoad("X");
			else
				context.EmitRegisterLoad("Y");

			context.EmitRegisterValueRetrieval<IntegerRegister>();
			context.EmitCastToIntPtr();
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			var opcode = GetDirectRuntimeOpcode();

			Register register = StringSerialization.ParseRegisterArgument(context, args[0]);

			if (opcode.IndexWithXRegister && !object.ReferenceEquals(register, Registers.X))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" expects register &X as the argument"));
			else if (!opcode.IndexWithXRegister && !object.ReferenceEquals(register, Registers.Y))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" expects register &Y as the argument"));

			// No arguments need to be saved
			return null;
		}

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) { }
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads an element from an array on the stack using the X or Y registers
	/// </summary>
	public abstract class LoadElementInArrayCompilingOpcode<TOpcode> : ArrayElementAccessCompilingOpcode<TOpcode> where TOpcode : LoadElementInArrayOpcode, new() {
		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			ValidateArgumentAndEmitInstructions(context);

			var opcode = GetDirectRuntimeOpcode();

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			if (opcode.LoadsAddress)
				context.EmitNopAndDelayedResolver<DelayedArrayLoadAddressResolver>();
			else
				context.EmitNopAndDelayedResolver<DelayedArrayLoadResolver>();
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that pops an array and value from the stack and stores the value to an element in an array using the X or Y registers
	/// </summary>
	public abstract class StoreToElementInArrayCompilingOpcode<TOpcode> : ArrayElementAccessCompilingOpcode<TOpcode> where TOpcode : StoreToElementInArrayOpcode, new() {
		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			ValidateArgumentAndEmitInstructions(context);

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			context.EmitNopAndDelayedResolver<DelayedArrayStoreResolver>();
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads or sets one of the flags in the F register
	/// </summary>
	public abstract class ModifyFlagsRegisterCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : ModifyFlagsRegisterOpcode, new() {
		public sealed override int ExpectedArgumentCount => 0;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			var opcode = GetDirectRuntimeOpcode();

			if (opcode.SetsFlag)
				context.EmitFlagAssignment(opcode.Flag, opcode.FlagValue);
			else
				context.EmitFlagRetrieval(opcode.Flag);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) { }
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that uses a type token as an argument
	/// </summary>
	public abstract class TypeOperandCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : TypeOperandOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) {
			var opcode = GetDirectRuntimeOpcode();

			var type = context.heap.ReadString(reader);

			if (type == "") {
				if (!opcode.AllowsNull)
					throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" does not allow null as an argument"));

				return new OpcodeArgumentCollection().Add(null);
			}

			var typeDef = reader.ReadTypeDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(typeDef);

			return new OpcodeArgumentCollection()
				.Add(typeDef);
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			var opcode = GetDirectRuntimeOpcode();

			if (args[0] == null) {
				if (!opcode.AllowsNull)
					throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" does not allow null as an argument"));

				return new OpcodeArgumentCollection().Add(null);
			}

			return new OpcodeArgumentCollection()
				.Add(new DelayedTypeResolver(context.resolver, args[0], false));
		}

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) {
			var opcode = GetDirectRuntimeOpcode();

			var arg = args[0];
			if (arg is null) {
				if (!opcode.AllowsNull)
					throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" does not allow null as an argument"));

				context.heap.WriteString(writer, "");
			} else if (arg is ITypeDefOrRef type)
				writer.Write(type, context.heap);
			else
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException("Argument was not an ITypeDefOrRef instance"));
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that uses a register as an argument
	/// </summary>
	public abstract class RegisterOperandCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : Opcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) {
			try {
				return new OpcodeArgumentCollection()
					.Add(Registers.GetRegisterFromID(reader.ReadByte()));
			} catch (Exception ex) {
				ChipsCompiler.ErrorAndThrow(ex);
				throw;
			}
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			if (args[0] is not string register)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException("Argument was not a string"));

			return new OpcodeArgumentCollection()
				.Add(StringSerialization.ParseRegisterArgument(context, register));
		}

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) {
			writer.Write((byte)args.GetValue<Register>(0).ID);
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that uses a label as an argument
	/// </summary>
	public abstract class LabelOperandCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : Opcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(CompilationContext context, BinaryReader reader) {
			int index = reader.ReadUInt16();

			return new OpcodeArgumentCollection()
				.Add(context.ActiveMethod.Labels[index]);
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			return new OpcodeArgumentCollection()
				.Add(args[0]);
		}

		public sealed override void SerializeArguments(CompilationContext context, BinaryWriter writer, OpcodeArgumentCollection args) {
			var label = context.ActiveMethod.FindLabel(args.GetValue<string>(0));

			writer.Write((ushort)label.Index);
		}
	}
}
