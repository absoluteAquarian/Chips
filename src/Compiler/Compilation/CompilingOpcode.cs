using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler.Utility;
using Chips.Runtime;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
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
		/// <param name="reader">The data stream</param>
		/// <param name="resolver">An object for resolving assemblies and types</param>
		/// <returns>A collection of arguments.  Return <see langword="null"/> to indicate that this opcode has no arguments</returns>
		public abstract OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap);

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
		/// <param name="writer">The data stream</param>
		/// <param name="args">The collection of arguments</param>
		/// <param name="resolver">An object for resolving assemblies and types</param>
		public abstract void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap);
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

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
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

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			try {
				return new OpcodeArgumentCollection()
					.Add(DeserializeArgument(reader, resolver, heap));
			} catch (Exception ex) {
				ChipsCompiler.ErrorAndThrow(ex);
				throw;
			}
		}

		protected abstract T DeserializeArgument(BinaryReader reader, TypeResolver resolver, StringHeap heap);

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

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			var registerName = GetDirectRuntimeOpcode().Register;

			Register register = typeof(Registers).RetrieveStaticField<Register>(registerName)!;

			var arg = ValidateArgument(args);
			
			if (!register.AcceptsValue(arg))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Register \"{registerName}\" does not accept values of type \"{arg?.GetType().GetSimplifiedGenericTypeName() ?? "null"}\""));

			try {
				SerializeArgument(writer, arg, resolver, heap);
			} catch (Exception ex) {
				ChipsCompiler.ErrorAndThrow(ex);
				throw;
			}
		}

		protected abstract void SerializeArgument(BinaryWriter writer, T arg, TypeResolver resolver, StringHeap heap);
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a "zero" to a register
	/// </summary>
	public abstract class LoadZeroCompilingOpcode<TOpcode> : BasicCompilingOpcode<TOpcode> where TOpcode : LoadZeroOpcode, new() {
		protected void EmitRegisterAccess(CompilationContext context) {
			context.EmitRegisterLoad(GetDirectRuntimeOpcode().Register);
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a field or field address to the stack
	/// </summary>
	public abstract class LoadFieldCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : LoadFieldOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			var opcode = GetDirectRuntimeOpcode();

			FieldDefinition fieldDefinition;
			if (args[0] is DelayedFieldResolver delayed)
				fieldDefinition = delayed.Member ?? throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" could not evaluate its argument"));
			else if (args[0] is FieldDefinition def)
				fieldDefinition = def;
			else
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" could not evaluate its argument"));

			// Field is static, but opcode expects an instance field or vice versa
			if (fieldDefinition.IsStatic != opcode.LoadsStaticField)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" expects an identifier for {(opcode.LoadsStaticField ? "a static" : "an instance")} field.  Field \"{fieldDefinition.Name}\" in type \"{fieldDefinition.DeclaringType!.Name}\" is {(fieldDefinition.IsStatic ? "a static" : "an instance")} field"));

			// Emit the field access
			var importedField = context.importer.ImportField(fieldDefinition);

			if (opcode.LoadsStaticField)
				context.Cursor.Emit(opcode.LoadsAddress ? CilOpCodes.Ldsflda : CilOpCodes.Ldsfld, importedField);
			else
				context.Cursor.Emit(opcode.LoadsAddress ? CilOpCodes.Ldflda : CilOpCodes.Ldfld, importedField);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			var field = reader.ReadFieldDefinition(resolver, heap);
			ChipsCompiler.AddDelayedResolver(field);

			return new OpcodeArgumentCollection()
				.Add(field);
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			return new OpcodeArgumentCollection()
				.Add(new DelayedFieldResolver(context.resolver, args[0]));
		}

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			var arg = args[0];
			if (arg is not IFieldDescriptor field)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException("Argument was not an IFieldDescriptor instance"));

			writer.Write(field, heap);
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads a method argument to the stack
	/// </summary>
	public abstract class LoadMethodVariableCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : LoadMethodVariableOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			var opcode = GetDirectRuntimeOpcode();

			if (args[0] is not ushort arg)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" expects an unsigned 16-bit integer as the argument"));

			if (opcode.LoadsLocal)
				context.Cursor.Emit(opcode.LoadsAddress ? CilOpCodes.Ldloca : CilOpCodes.Ldloc, arg);
			else
				context.Cursor.Emit(opcode.LoadsAddress ? CilOpCodes.Ldarga : CilOpCodes.Ldarg, arg);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			return new OpcodeArgumentCollection()
				.Add(reader.ReadUInt16());
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			return new OpcodeArgumentCollection()
				.Add(StringSerialization.ParseSimpleSmallIntegerArgument(context, args[0]));
		}

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			writer.Write((ushort)args[0]!);
		}
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that loads an element from an array on the stack using the X or Y registers
	/// </summary>
	public abstract class LoadElementInArrayCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : LoadElementInArrayOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			var opcode = GetDirectRuntimeOpcode();

			if (opcode.IndexWithXRegister)
				context.EmitRegisterLoad("X");
			else
				context.EmitRegisterLoad("Y");

			context.EmitRegisterValueRetrieval<IntegerRegister>();
			context.EmitCastTo<IntPtr_T>();
			context.EmitUnderlyingTypeValueRetrieval<IntPtr_T>();

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			// Variable capturing
			bool loadsAddress = opcode.LoadsAddress;
			context.EmitNopAndDelayedResolver((body, index) => loadsAddress ? new DelayedArrayLoadAddressResolver(body, index) : new DelayedArrayLoadResolver(body, index));
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

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

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
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

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}

	/// <summary>
	/// An implementation of <see cref="CompilingOpcode"/> which represents an instruction that uses a type token as an argument
	/// </summary>
	public abstract class TypeOperandCompilingOpcode<TOpcode> : CompilingOpcode<TOpcode> where TOpcode : TypeOperandOpcode, new() {
		public sealed override int ExpectedArgumentCount => 1;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			var opcode = GetDirectRuntimeOpcode();

			var type = heap.ReadString(reader);

			if (type == "") {
				if (!opcode.AllowsNull)
					throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" does not allow null as an argument"));

				return new OpcodeArgumentCollection().Add(null);
			}

			var typeDef = reader.ReadTypeDefinition(resolver, heap);
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

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			var opcode = GetDirectRuntimeOpcode();

			var arg = args[0];
			if (arg is null) {
				if (!opcode.AllowsNull)
					throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{opcode.Name}\" does not allow null as an argument"));

				heap.WriteString(writer, "");
			} else if (arg is ITypeDefOrRef type)
				writer.Write(type, heap);
			else
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException("Argument was not an ITypeDefOrRef instance"));
		}
	}
}
