using Chips.Common.Utility;
using Chips.Runtime.Meta;
using Chips.Runtime.Utility;
using System;

namespace Chips.Runtime {
	public class UninitializedRegisterException(Register register) : Exception($"Register {register.GetName()} has not been assigned a value.") { }

	public class InvalidRegisterException<T>(Register register) : Exception($"Register {register.GetName()} does not support values of type \"{typeof(T).GetFullGenericTypeName()}\".") { }

	public class RegisterMismatchException<T>(Register register) : Exception($"Register {register.GetName()} did not contain a value of type \"{typeof(T).GetFullGenericTypeName()}\".") { }

	public class InvalidTypeToRegisterEnumException<TEnum, TType> : Exception where TEnum : struct, Enum {
		public InvalidTypeToRegisterEnumException() : base($"Type \"{typeof(TType).GetFullGenericTypeName()}\" is not a valid type for register classification \"{typeof(TEnum).Name.Trim('_')}\".") { }
	}

	public class InvalidRegisterMethodException(Register register, string alternative) : Exception($"Register {register.GetName()} does not support the current method.  Use {alternative}() instead.") { }
}
