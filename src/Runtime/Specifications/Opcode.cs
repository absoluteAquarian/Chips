using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler;
using Chips.Compiler.Utility;
using Chips.Parsing;
using Chips.Runtime.Types;
using Chips.Runtime.Utility;
using Chips.Utility;
using Chips.Utility.Reflection;
using Sprache;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

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
		/// The expected evaluation stack behavior for this Chips instruction
		/// </summary>
		public abstract StackBehavior StackBehavior { get; }

		/// <summary>
		/// Add the CIL instructions used when transpiling this Chips instruction here
		/// </summary>
		/// <param name="context">An object containing information about the current method being compiled</param>
		/// <param name="args">The arguments used for this instruction</param>
		/// <remarks>If no instructions are added, the default implementation of loading the static opcode field and calling its function is used instead</remarks>
		public virtual bool Compile(CompilationContext context, OpcodeArgumentCollection args) => true;

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
		public abstract OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver);

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
		public abstract void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver);
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> that has no arguments with the stack behavior forced to <see cref="StackBehavior.None"/>
	/// </summary>
	public abstract class BasicOpcode : Opcode {
		public sealed override StackBehavior StackBehavior => StackBehavior.None;

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver) { }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a constant value to a register
	/// </summary>
	public abstract class LoadConstantOpcode : Opcode {
		public abstract string Register { get; }

		public sealed override StackBehavior StackBehavior => StackBehavior.None;

		protected bool ValidateArgumentAndEmitRegisterAccess<T>(CompilationContext context, OpcodeArgumentCollection args, [NotNullWhen(true)] out T? arg, string expectedArgument) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			if (ValueConverter.BoxToUnderlyingType(args[0]) is not T argAsType)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects {expectedArgument} argument"));

			context.Instructions.Add(CilOpCodes.Ldsfld, context.importer.ImportField(typeof(Registers).GetCachedField(Register)!));

			arg = argAsType;
			return true;
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver) {
			try {
				return new OpcodeArgumentCollection()
					.Add(DeserializeArgument(reader));
			} catch (Exception ex) {
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, ex);
			}
		}

		protected abstract object? DeserializeArgument(BinaryReader reader);

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			if (args.Length != 1)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			try {
				return new OpcodeArgumentCollection()
					.Add(ParseArgument(context, args[0]));
			} catch (Exception ex) {
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, ex);
			}
		}

		protected abstract object? ParseArgument(CompilationContext context, string arg);

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver) {
			if (typeof(Registers).RetrieveStaticField<Register>(Register) is not Register register)
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, new InvalidOperationException($"Register \"{Register}\" does not exist"));

			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expected one argument, received {args.Count}"));

			var arg = args[0];
			if (!register.AcceptsValue(arg))
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, new ArgumentException($"Register \"{Register}\" does not accept values of type \"{arg?.GetType().GetSimplifiedGenericTypeName() ?? "null"}\""));

			try {
				SerializeArgument(writer, arg);
			} catch (Exception ex) {
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, ex);
			}
		}

		protected abstract void SerializeArgument(BinaryWriter writer, object? arg);
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a field or field address to the stack
	/// </summary>
	public abstract class LoadFieldOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool LoadsStaticField { get; }

		public sealed override StackBehavior StackBehavior => LoadsStaticField ? StackBehavior.PushOne : StackBehavior.PopOne | StackBehavior.PushOne;

		public override bool Compile(CompilationContext context, OpcodeArgumentCollection args) {
			return ValidateArgumentsAndEmitFieldAccess(context, args, out _);
		}

		protected bool ValidateArgumentsAndEmitFieldAccess(CompilationContext context, OpcodeArgumentCollection args, [NotNullWhen(true)] out FieldDefinition? field) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects one argument"));

			if (args[0] is not FieldDefinition fieldDefinition)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" could not evaluate its argument"));

			// Field is static, but opcode expects an instance field or vice versa
			if (fieldDefinition.IsStatic != LoadsStaticField)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Opcode \"{Name}\" expects an identifier for {(LoadsStaticField ? "a static" : "an instance")} field.  Field \"{fieldDefinition.Name}\" in type \"{fieldDefinition.DeclaringType!.Name}\" is {(fieldDefinition.IsStatic ? "a static" : "an instance")} field"));

			// Emit the field access
			if (LoadsStaticField) {
				if (LoadsAddress)
					context.Instructions.Add(CilOpCodes.Ldsflda, context.importer.ImportField(fieldDefinition));
				else
					context.Instructions.Add(CilOpCodes.Ldsfld, context.importer.ImportField(fieldDefinition));
			} else {
				if (LoadsAddress)
					context.Instructions.Add(CilOpCodes.Ldflda, context.importer.ImportField(fieldDefinition));
				else
					context.Instructions.Add(CilOpCodes.Ldfld, context.importer.ImportField(fieldDefinition));
			}

			field = fieldDefinition;
			return true;
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver) {
			return new OpcodeArgumentCollection()
				.Add(reader.ReadFieldDefinition(resolver));
		}

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) {
			if (args.Length != 1)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new ArgumentException($"Expected one argument, received {args.Length}"));

			return new OpcodeArgumentCollection()
				.Add(ParseFieldIdentifierArgument(context, args[0]));
		}

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver) {
			if (args.Count != 1)
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, new ArgumentException($"Expected one argument, received {args.Count}"));

			var arg = args[0];
			if (arg is not FieldDefinition fieldDefinition)
				throw ChipsCompiler.ErrorAndThrow(resolver.activeSourceFile, new ArgumentException("Argument was not a FieldDefinition instance"));

			writer.Write(fieldDefinition, resolver);
		}
	}
}
