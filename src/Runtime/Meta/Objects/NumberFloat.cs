using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _NumberFloat : IChipsObject, ICreateObject<_NumberFloat>, IConvertToFloat<_NumberFloat> {
		public _FloatData data;
		public _Float type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _NumberFloat IConvertToFloat<_NumberFloat>.AsFloat(in _NumberFloat value, Span<_NumberFloat> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _NumberFloat ICreateObject<_NumberFloat>.Create<U>(in U value) {
			if (typeof(U) == typeof(_NumberFloat))
				return Unsafe.As<U, _NumberFloat>(ref Unsafe.AsRef(in value));

			_NumberFloat obj = default;
			obj.type = TypeMetrics.TypeToFloat<U>();
			Unsafe.As<_NumberFloat, U>(ref obj) = value;
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = sizeof(decimal))]
	internal struct _FloatData {
		[FieldOffset(0)] public float Single;
		[FieldOffset(0)] public double Double;
		[FieldOffset(0)] public decimal Decimal;
		[FieldOffset(0)] public Half Half;
	}

	internal enum _Float {
		Single,
		Double,
		Decimal,
		Half
	}
}
