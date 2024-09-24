using Chips.Common.Utility;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	internal ref struct TypedRegister<T> {
		public ref T value;
		public readonly Register register;

		public TypedRegister(Register register, ref T value) {
			if (typeof(T) != typeof(_NumberInteger) && typeof(T) != typeof(_Address) && typeof(T) != typeof(_NumberFloat)
			&& typeof(T) != typeof(_StringObject) && typeof(T) != typeof(_Object) && typeof(T) != typeof(_VariantObject)
			&& typeof(T) != typeof(_VectorObject) && typeof(T) != typeof(_TokenHandle)
			&& typeof(T) != typeof(Status)) {
				if (!typeof(T).IsAssignableFrom(typeof(Exception)))
					throw new NotSupportedException($"Type {typeof(T).GetFullGenericTypeName()} is not supported");
			}

			this.register = register;
			this.value = ref value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly InvalidRegisterStateException MalformedArgument() => new InvalidRegisterStateException(register);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly InvalidRegisterTypeException InvalidType() => new InvalidRegisterTypeException(register);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly InvalidNumberOperandException<TNumber> InvalidNumberType<TNumber>() where TNumber : INumberBase<TNumber> => new InvalidNumberOperandException<TNumber>(register);
	}
}
