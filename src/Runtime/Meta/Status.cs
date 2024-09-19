using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	public partial struct Status {
		private ushort word;

		private const int MASK_CARRY = 0x0001;
		private const int MASK_OVERFLOW = 0x0002;
		private const int MASK_NEGATIVE = 0x0004;
		private const int MASK_ZERO = 0x0008;
		private const int MASK_NAN = 0x0010;

		public bool Carry {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (word & MASK_CARRY) != 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => word = (ushort)((word & ~MASK_CARRY) | (value ? MASK_CARRY : 0));
		}

		public bool Overflow {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (word & MASK_OVERFLOW) != 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => word = (ushort)((word & ~MASK_OVERFLOW) | (value ? MASK_OVERFLOW : 0));
		}

		public bool Negative {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (word & MASK_NEGATIVE) != 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => word = (ushort)((word & ~MASK_NEGATIVE) | (value ? MASK_NEGATIVE : 0));
		}

		public bool Zero {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (word & MASK_ZERO) != 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => word = (ushort)((word & ~MASK_ZERO) | (value ? MASK_ZERO : 0));
		}

		public bool NaN {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (word & MASK_NAN) != 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => word = (ushort)((word & ~MASK_NAN) | (value ? MASK_NAN : 0));
		}
	}
}
