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
}
