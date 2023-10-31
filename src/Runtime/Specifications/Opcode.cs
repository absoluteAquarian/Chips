using AsmResolver.PE.DotNet.Cil;
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

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a constant value to a register
	/// </summary>
	public abstract class LoadConstantOpcode<T> : Opcode {
		public abstract string Register { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a "zero" to a register
	/// </summary>
	public abstract class LoadZeroOpcode : Opcode {
		public abstract string Register { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a field or field address to the stack
	/// </summary>
	public abstract class LoadFieldOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool LoadsStaticField { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads a method argument to the stack
	/// </summary>
	public abstract class LoadMethodVariableOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool LoadsLocal { get; }
	}

	/// <summary>
	/// An implementation of <see cref="Opcode"/> which represents an instruction that loads an element from an array on the stack using the X or Y registers
	/// </summary>
	public abstract class LoadElementInArrayOpcode : Opcode {
		public abstract bool LoadsAddress { get; }

		public abstract bool IndexWithXRegister { get; }
	}

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
}
