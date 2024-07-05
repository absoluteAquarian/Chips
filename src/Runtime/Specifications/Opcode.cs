using AsmResolver.PE.DotNet.Cil;
using Chips.Runtime.Types;
using System;

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
					name = name[..index];
				return name.ToLower();
			}
		}

		/// <summary>
		/// An optional address to this instruction's method.<br/>
		/// If a custom <see cref="Compile"/> implementation is used, this property should be <see cref="nint.Zero"/>
		/// </summary>
		public virtual unsafe nint Method => nint.Zero;

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
	}

	public interface IConstantOperandOpcode<T> {
		Register Register { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a constant value to a register
	/// </summary>
	public abstract class LoadConstantOpcode<T> : Opcode, IConstantOperandOpcode<T> {
		public abstract Register Register { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a "zero" to a register
	/// </summary>
	public abstract class LoadZeroOpcode : Opcode {
		public abstract string Register { get; }
	}

	public abstract class FieldAccessOpcode : Opcode {
		public abstract bool AccessesStaticField { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a field or field address to the stack
	/// </summary>
	public abstract class LoadFieldOpcode : FieldAccessOpcode {
		public abstract bool LoadsAddress { get; }

		public sealed override bool AccessesStaticField => LoadsStaticField;

		public abstract bool LoadsStaticField { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that pops a value from the stack and stores it in a field
	/// </summary>
	public abstract class StoreToFieldOpcode : FieldAccessOpcode {
		public sealed override bool AccessesStaticField => StoresToStaticField;

		public abstract bool StoresToStaticField { get; }
	}

	public abstract class MethodVariableAccessOpcode : Opcode {
		public abstract bool AccessesLocal { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a method argument or local to the stack
	/// </summary>
	public abstract class LoadMethodVariableOpcode : MethodVariableAccessOpcode {
		public abstract bool LoadsAddress { get; }

		public sealed override bool AccessesLocal => LoadsLocal;

		public abstract bool LoadsLocal { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that pops a value from the stack and stores it in a method argument or local
	/// </summary>
	public abstract class StoreToMethodVariableOpcode : MethodVariableAccessOpcode {
		public sealed override bool AccessesLocal => StoresToLocal;

		public abstract bool StoresToLocal { get; }
	}

	public abstract class ArrayElementAccessOpcode : Opcode {
		public abstract bool IndexWithXRegister { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads an element from an array on the stack using the X or Y registers
	/// </summary>
	public abstract class LoadElementInArrayOpcode : ArrayElementAccessOpcode {
		public abstract bool LoadsAddress { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that pops a value from the stack and stores it in an element in an array using the X or Y registers
	/// </summary>
	public abstract class StoreToElementInArrayOpcode : ArrayElementAccessOpcode { }

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads or sets one of the flags in the F register
	/// </summary>
	public abstract class ModifyFlagsRegisterOpcode : Opcode {
		public abstract bool SetsFlag { get; }

		public abstract bool FlagValue { get; }

		public abstract string Flag { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that uses a type token as an argument
	/// </summary>
	public abstract class TypeOperandOpcode : Opcode {
		public abstract bool AllowsNull { get; }
	}

	public abstract class ArithmeticOpcode : Opcode {
		public abstract ArithmeticOperation Operation { get; }

		public abstract Register Register { get; }
	}

	public abstract class ArithmeticWithOperandOpcode<T> : ArithmeticOpcode, IConstantOperandOpcode<T> { }
}
