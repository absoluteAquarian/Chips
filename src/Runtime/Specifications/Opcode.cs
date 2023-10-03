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
using System.IO;

namespace Chips.Runtime.Specifications {
	/// <summary>
	/// The base type for all Chips instructions
	/// </summary>
	public abstract partial class Opcode {
		/// <summary>
		/// The byte representation for this Chips instruction
		/// </summary>
		public abstract OpcodeID Code { get; }

		public string Name {
			get {
				string name = Code.ToString();
				int index = name.IndexOf('_');
				if (index != -1)
					name = name[(index + 1)..];
				return name.ToLower();
			}
		}

		/// <summary>
		/// An optional address to this instruction's method.<br/>
		/// If a custom <see cref="Compile"/> implementation is used, this property should be <see cref="nint.Zero"/>
		/// </summary>
		public virtual nint Method => nint.Zero;

		/// <summary>
		/// The expected stack behavior for this Chips instruction
		/// </summary>
		public abstract StackBehavior StackBehavior { get; }

		/// <summary>
		/// The expected number of arguments to be given to this instructino
		/// </summary>
		public abstract int ExpectedArgumentCount { get; }

		/// <summary>
		/// Add the CIL instructions used when transpiling this Chips instruction here
		/// </summary>
		/// <param name="context">An object containing information about the current method being compiled</param>
		/// <param name="args">The arguments used for this instruction</param>
		/// <remarks>If no instructions are added, the default implementation of loading the static opcode field and calling its function is used instead</remarks>
		public virtual void Compile(CompilationContext context, OpcodeArgumentCollection args) { }

		/// <summary>
		/// Gets the return type and parameter types used for this Chips instruction's method.<br/>
		/// These types are used to construct the <see cref="CilOpCodes.Calli"/> instruction
		/// </summary>
		/// <param name="returnType">The type returned by the method.  Defaults to <see langword="typeof"/>(<see langword="void"/>), which represents a method that does not return anything.</param>
		/// <param name="parameterTypes">The types of the parameters for the method.  Defaults to <see cref="Type.EmptyTypes"/>, which represents a method with no arguments.</param>
		public virtual void GetMethodSignature(out Type returnType, out Type[] parameterTypes) {
			returnType = typeof(void);
			parameterTypes = Type.EmptyTypes;
		}

		/// <summary>
		/// Gets the expected evaluation stack modification for this opcode.  Defaults to reading <see cref="StackBehavior"/> and throws if <see cref="StackBehavior.PopVar"/> is used
		/// </summary>
		/// <param name="popped">How many values are popped</param>
		/// <param name="pushed">How many values are pushed.</param>
		public virtual void GetStackModification(out int popped, out int pushed) {
			var behaviour = StackBehavior;

			if ((behaviour & StackBehavior.PopVar) != 0)
				throw new InvalidOperationException("Cannot get stack modification for an opcode with variable stack behavior");

			popped = ((int)behaviour & 0xFF00) >> 8;
			pushed = (int)behaviour & 0xFF;
		}

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
	/// An implementation of <see cref="Opcode"/> that has no arguments with the stack behavior forced to <see cref="StackBehavior.None"/>
	/// </summary>
	public abstract class BasicOpcode : Opcode {
		public sealed override StackBehavior StackBehavior => StackBehavior.None;

		public sealed override int ExpectedArgumentCount => 0;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a constant value to a register
	/// </summary>
	public abstract class LoadConstantOpcode<T> : Opcode {
		public abstract string Register { get; }

		public sealed override StackBehavior StackBehavior => StackBehavior.None;

		public sealed override int ExpectedArgumentCount => 1;

		private T ValidateArgument(OpcodeArgumentCollection args) {
			if (args[0] is not T arg)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects a {typeof(T).GetSimplifiedGenericTypeName()} argument, received \"{args[0]?.GetType().GetSimplifiedGenericTypeName() ?? "null"}\" instead"));

			return arg;
		}

		protected void EmitRegisterAccess(CompilationContext context, OpcodeArgumentCollection args, out T arg) {
			arg = ValidateArgument(args);

			context.EmitRegisterLoad(Register);
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
			Register register = typeof(Registers).RetrieveStaticField<Register>(Register)!;

			var arg = ValidateArgument(args);
			
			if (!register.AcceptsValue(arg))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Register \"{Register}\" does not accept values of type \"{arg?.GetType().GetSimplifiedGenericTypeName() ?? "null"}\""));

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
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a "zero" to a register
	/// </summary>
	public abstract class LoadZeroOpcode : BasicOpcode {
		public abstract string Register { get; }

		protected void EmitRegisterAccess(CompilationContext context) {
			context.EmitRegisterLoad(Register);
		}
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a field or field address to the stack
	/// </summary>
	public abstract class LoadFieldOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool LoadsStaticField { get; }

		public sealed override StackBehavior StackBehavior => LoadsStaticField ? StackBehavior.PushOne : StackBehavior.PopOne | StackBehavior.PushOne;

		public sealed override int ExpectedArgumentCount => 1;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			FieldDefinition fieldDefinition;
			if (args[0] is DelayedFieldResolver delayed)
				fieldDefinition = delayed.Member ?? throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" could not evaluate its argument"));
			else if (args[0] is FieldDefinition def)
				fieldDefinition = def;
			else
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" could not evaluate its argument"));

			// Field is static, but opcode expects an instance field or vice versa
			if (fieldDefinition.IsStatic != LoadsStaticField)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects an identifier for {(LoadsStaticField ? "a static" : "an instance")} field.  Field \"{fieldDefinition.Name}\" in type \"{fieldDefinition.DeclaringType!.Name}\" is {(fieldDefinition.IsStatic ? "a static" : "an instance")} field"));

			if (!LoadsStaticField) {
				var obj = context.stack.Pop();
				if (obj != StackObject.Object)
					throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects an object on the stack, received \"{obj}\" instead"));
			}

			// Emit the field access
			var importedField = context.importer.ImportField(fieldDefinition);

			if (LoadsStaticField)
				context.Instructions.Add(LoadsAddress ? CilOpCodes.Ldsflda : CilOpCodes.Ldsfld, importedField);
			else
				context.Instructions.Add(LoadsAddress ? CilOpCodes.Ldflda : CilOpCodes.Ldfld, importedField);

			context.stack.Push(LoadsAddress ? StackObject.Address : StackObject.Object);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			var field = reader.ReadFieldDefinition(resolver, heap);
			ChipsCompiler.AddDelayedResolver(field);

			return new OpcodeArgumentCollection()
				.Add(field);
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			if (args.Length != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Expected one argument, received {args.Length}"));

			var field = new DelayedFieldResolver(context.resolver, args[0]);
			ChipsCompiler.AddDelayedResolver(field);

			return new OpcodeArgumentCollection()
				.Add(field);
		}

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Expected one argument, received {args.Count}"));

			var arg = args[0];
			if (arg is not IFieldDescriptor field)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException("Argument was not an IFieldDescriptor instance"));

			writer.Write(field, heap);
		}
	}

	public abstract class LoadMethodVariableOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool LoadsLocal { get; }

		public sealed override StackBehavior StackBehavior => StackBehavior.PushOne;

		public sealed override int ExpectedArgumentCount => 1;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (args[0] is not ushort arg)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects an unsigned 16-bit integer as the argument"));

			if (LoadsLocal)
				context.Instructions.Add(LoadsAddress ? CilOpCodes.Ldloca : CilOpCodes.Ldloc, arg);
			else
				context.Instructions.Add(LoadsAddress ? CilOpCodes.Ldarga : CilOpCodes.Ldarg, arg);

			context.stack.Push(LoadsAddress ? StackObject.Address : StackObject.Object);
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

	public abstract class LoadElementInArrayOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool IndexWithXRegister { get; }

		public sealed override StackBehavior StackBehavior => StackBehavior.PushOne | StackBehavior.PopOne;

		public sealed override int ExpectedArgumentCount => 1;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			StackObject obj = context.stack.Pop();

			if (obj is not StackObject.Object)
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects an object on the stack, received \"{obj}\" instead"));

			if (IndexWithXRegister)
				context.EmitRegisterLoad("X");
			else
				context.EmitRegisterLoad("Y");

			context.EmitRegisterValueRetrieval<IntegerRegister>();
			context.EmitCastTo<IntPtr_T>();
			context.EmitUnderlyingTypeValueRetrieval<IntPtr_T>();

			// Instruction will be delayed in order to ensure that the stack is set up properly for later instructions
			// Variable capturing
			bool loadsAddress = LoadsAddress;
			context.EmitDelayedResolver((body, index) => loadsAddress ? new DelayedArrayLoadAddressResolver(body, index) : new DelayedArrayLoadResolver(body, index));

			context.stack.Push(LoadsAddress ? StackObject.Address : StackObject.Object);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			Register register = StringSerialization.ParseRegisterArgument(context, args[0]);

			if (IndexWithXRegister && !object.ReferenceEquals(register, Registers.X))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects register &X as the argument"));
			else if (!IndexWithXRegister && !object.ReferenceEquals(register, Registers.Y))
				throw ChipsCompiler.ErrorAndThrow(new ArgumentException($"Opcode \"{Name}\" expects register &Y as the argument"));

			// No arguments need to be saved
			return null;
		}

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}

	public abstract class ModifyFlagsRegisterOpcode : Opcode {
		public abstract bool SetsFlag { get; }

		public abstract bool FlagValue { get; }

		public abstract string Flag { get; }

		public sealed override StackBehavior StackBehavior => SetsFlag ? StackBehavior.None : StackBehavior.PushOne;

		public sealed override int ExpectedArgumentCount => 0;

		public sealed override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			if (SetsFlag)
				context.EmitFlagAssignment(Flag, FlagValue);
			else {
				context.EmitFlagRetrieval(Flag);

				context.stack.Push(StackObject.Boolean);
			}
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}
}
