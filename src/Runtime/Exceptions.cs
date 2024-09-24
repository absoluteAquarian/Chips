using Chips.Common.Utility;
using Chips.Runtime.Meta;
using Chips.Runtime.Utility;
using System;
using System.Numerics;

namespace Chips.Runtime {
	public class UninitializedRegisterException(Register register) : Exception($"Register {register.GetName()} has not been assigned a value.") { }

	public class InvalidRegisterException<T>(Register register) : Exception($"Register {register.GetName()} does not support values of type \"{typeof(T).GetFullGenericTypeName()}\".") { }

	public class RegisterMismatchException<T>(Register register) : Exception($"Register {register.GetName()} did not contain a value of type \"{typeof(T).GetFullGenericTypeName()}\".") { }

	public class InvalidTypeToRegisterEnumException<TEnum, TType> : Exception where TEnum : struct, Enum {
		public InvalidTypeToRegisterEnumException() : base($"Type \"{typeof(TType).GetFullGenericTypeName()}\" is not a valid type for register classification \"{typeof(TEnum).Name.Trim('_')}\".") { }
	}

	public class InvalidRegisterMethodException(Register register, string alternative) : Exception($"Register {register.GetName()} does not support the current method.  Use {alternative}() instead.") { }

	public class NotAVariantRegisterException(Register register) : Exception($"Register {register.GetName()} is not a variant object register.") { }

	public class NotAnAddressRegisterException(Register register) : Exception($"Register {register.GetName()} is not an address register.") { }

	public class NotAnIntegerRegisterException(Register register) : Exception($"Register {register.GetName()} is not an integer register.") { }

	public class NotAFloatRegisterException(Register register) : Exception($"Register {register.GetName()} is not a floating-point register.") { }

	public class NotAStringRegisterException(Register register) : Exception($"Register {register.GetName()} is not a string register.") { }

	public class NotAnObjectRegisterException(Register register) : Exception($"Register {register.GetName()} is not an object register.") { }

	public class NotAVectorRegisterException(Register register) : Exception($"Register {register.GetName()} is not a vector register.") { }

	public class NotATokenRegisterException(Register register) : Exception($"Register {register.GetName()} is not a metadata token register.") { }

	public class NotAnExceptionRegisterException(Register register) : Exception($"Register {register.GetName()} is not an exception register.") { }

	public class InvalidRegisterTypeException(Register register) : Exception($"Register {register.GetName()} contained a value which does not support the current operation.") { }

	internal class InvalidVariantObjectTypeException : Exception {
		public InvalidVariantObjectTypeException(in _VariantObject obj, _Variant expected) : base($"Variant object contained a value of type \"{obj.type}\" which does not match the expected type \"{expected}\".") { }

		public InvalidVariantObjectTypeException(in _VariantObject obj, _Variant expected, _Variant expected2) : base($"Variant object contained a value of type \"{obj.type}\" which does not match the expected types \"{expected}\" or \"{expected2}\".") { }
	}

	public class InvalidNumberOperandException<T> : Exception where T : INumberBase<T> {
		public InvalidNumberOperandException() : base($"The operand type \"{typeof(T).GetFullGenericTypeName()}\" is not a valid number type for the current operation.") { }

		public InvalidNumberOperandException(Register register) : base($"Register {register.GetName()} contained a value of type \"{typeof(T).GetFullGenericTypeName()}\" which is not a valid number type for the current operation.") { }
	}

	public class InvalidVectorTypeException<T>() : Exception($"The operand type \"{typeof(T).GetFullGenericTypeName()}\" is not a valid vector type for the current operation.") { }

	public class VectorOperandMismatchException<T>(Register register) : Exception($"Vector type \"{typeof(T).GetFullGenericTypeName()}\" does not match the vector type in register {register.GetName()}") { }

	public class VectorOperandMismatchException<TSource, TOperand>() : Exception($"Operand vector type \"{typeof(TOperand).GetFullGenericTypeName()}\" does not match expected vector type \"{typeof(TSource).GetFullGenericTypeName()}\"") { }

	public class InvalidTargetException : Exception {
		public InvalidTargetException(Register target) : base($"Register {target.GetName()} cannot be used as the target for the current operation.") { }

		public InvalidTargetException(Register target, Register operand) : base($"Register {target.GetName()} cannot be used as the target for the current operation with register {operand.GetName()}") { }

		public InvalidTargetException(Register target, Register first, Register second) : base($"Register {target.GetName()} cannot be used as the target for the current operation with registers {first.GetName()} and {second.GetName()}") { }
	}

	public class InvalidConversionException<T>(string classification) : ArgumentException($"The type \"{typeof(T).GetFullGenericTypeName()}\" is not a valid {classification} type.") { }

	public class CannotConvertTypeException<T>(string classification) : ArgumentException($"The type \"{typeof(T).GetFullGenericTypeName()}\" cannot be converted to the {classification} type.") { }

	public class InvalidObjectStateException(string paramName) : ArgumentException("The provided object was in an invalid state.", paramName) { }

	public class InvalidRegisterStateException(Register register) : Exception($"Register {register.GetName()} contained an object in an invalid state.") { }
}
