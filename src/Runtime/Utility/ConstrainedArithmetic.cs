﻿using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Utility {
	partial class ConstrainedArithmetic {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static SByte AddWithOverflowCheck(SByte a, SByte b, out bool overflowFlag) {
            unchecked {
                SByte c = (SByte)(a + b);
                overflowFlag = ((SByte)(a ^ b) >= 0) & ((SByte)(a ^ c) < 0);
                return c;
            }
        }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte Add(SByte a, SByte b) {
			if (b < 0 && b > SByte.MinValue)
				return Subtract(a, (SByte)(-b));

			SByte sum = AddWithOverflowCheck(a, b, out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte Subtract(SByte a, SByte b) {
			if (b < 0 && b > SByte.MinValue)
				return Add(a, (SByte)(-b));

			SByte difference = AddWithOverflowCheck(a, (SByte)(-b), out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Int16 AddWithOverflowCheck(Int16 a, Int16 b, out bool overflowFlag) {
            unchecked {
                Int16 c = (Int16)(a + b);
                overflowFlag = ((Int16)(a ^ b) >= 0) & ((Int16)(a ^ c) < 0);
                return c;
            }
        }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16 Add(Int16 a, Int16 b) {
			if (b < 0 && b > Int16.MinValue)
				return Subtract(a, (Int16)(-b));

			Int16 sum = AddWithOverflowCheck(a, b, out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16 Subtract(Int16 a, Int16 b) {
			if (b < 0 && b > Int16.MinValue)
				return Add(a, (Int16)(-b));

			Int16 difference = AddWithOverflowCheck(a, (Int16)(-b), out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Int32 AddWithOverflowCheck(Int32 a, Int32 b, out bool overflowFlag) {
            unchecked {
                Int32 c = (Int32)(a + b);
                overflowFlag = ((Int32)(a ^ b) >= 0) & ((Int32)(a ^ c) < 0);
                return c;
            }
        }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32 Add(Int32 a, Int32 b) {
			if (b < 0 && b > Int32.MinValue)
				return Subtract(a, (Int32)(-b));

			Int32 sum = AddWithOverflowCheck(a, b, out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32 Subtract(Int32 a, Int32 b) {
			if (b < 0 && b > Int32.MinValue)
				return Add(a, (Int32)(-b));

			Int32 difference = AddWithOverflowCheck(a, (Int32)(-b), out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Int64 AddWithOverflowCheck(Int64 a, Int64 b, out bool overflowFlag) {
            unchecked {
                Int64 c = (Int64)(a + b);
                overflowFlag = ((Int64)(a ^ b) >= 0) & ((Int64)(a ^ c) < 0);
                return c;
            }
        }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64 Add(Int64 a, Int64 b) {
			if (b < 0 && b > Int64.MinValue)
				return Subtract(a, (Int64)(-b));

			Int64 sum = AddWithOverflowCheck(a, b, out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64 Subtract(Int64 a, Int64 b) {
			if (b < 0 && b > Int64.MinValue)
				return Add(a, (Int64)(-b));

			Int64 difference = AddWithOverflowCheck(a, (Int64)(-b), out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte Add(Byte a, Byte b) {
			Byte sum = (Byte)(a + b);

			if (sum < a) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte Subtract(Byte a, Byte b) {
			Byte difference = (Byte)(a - b);

			if (difference > a) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16 Add(UInt16 a, UInt16 b) {
			UInt16 sum = (UInt16)(a + b);

			if (sum < a) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16 Subtract(UInt16 a, UInt16 b) {
			UInt16 difference = (UInt16)(a - b);

			if (difference > a) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32 Add(UInt32 a, UInt32 b) {
			UInt32 sum = (UInt32)(a + b);

			if (sum < a) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32 Subtract(UInt32 a, UInt32 b) {
			UInt32 difference = (UInt32)(a - b);

			if (difference > a) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64 Add(UInt64 a, UInt64 b) {
			UInt64 sum = (UInt64)(a + b);

			if (sum < a) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64 Subtract(UInt64 a, UInt64 b) {
			UInt64 difference = (UInt64)(a - b);

			if (difference > a) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IntPtr AddWithOverflowCheck(IntPtr a, IntPtr b, out bool overflowFlag) {
            unchecked {
                IntPtr c = (IntPtr)(a + b);
                overflowFlag = ((IntPtr)(a ^ b) >= 0) & ((IntPtr)(a ^ c) < 0);
                return c;
            }
        }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr Add(IntPtr a, IntPtr b) {
			if (b < 0 && b > IntPtr.MinValue)
				return Subtract(a, (IntPtr)(-b));

			IntPtr sum = AddWithOverflowCheck(a, b, out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr Subtract(IntPtr a, IntPtr b) {
			if (b < 0 && b > IntPtr.MinValue)
				return Add(a, (IntPtr)(-b));

			IntPtr difference = AddWithOverflowCheck(a, (IntPtr)(-b), out bool overflow);

			if (overflow) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UIntPtr Add(UIntPtr a, UIntPtr b) {
			UIntPtr sum = (UIntPtr)(a + b);

			if (sum < a) {
				Registers.F.Overflow = true;
				return sum;
			}

			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UIntPtr Subtract(UIntPtr a, UIntPtr b) {
			UIntPtr difference = (UIntPtr)(a - b);

			if (difference > a) {
				Registers.F.Overflow = true;
				return difference;
			}

			return difference;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single Add(Single a, Single b) {
			if (b < 0)
				return Subtract(a, -b);

			// Float arithmetic doesn't overflow, but it does follow IEEE 754 rules.  Notably:
			//   NaN       + anything  = NaN
			//   anything  + NaN       = NaN
			//   Infinity  + not NaN   = Infinity
			//   not NaN   + Infinity  = Infinity
			//   -Infinity + not NaN  = -Infinity
			//   not NaN   + -Infinity = -Infinity
			return a + b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single Subtract(Single a, Single b) {
			if (b < 0)
				return Add(a, -b);

			// Float arithmetic doesn't overflow, but it does follow IEEE 754 rules.  Notably:
			//   NaN       - anything  = NaN
			//   anything  - NaN       = NaN
			//   Infinity  - not NaN   = Infinity
			//   not NaN   - Infinity  = Infinity
			//   -Infinity - not NaN  = -Infinity
			//   not NaN   - -Infinity = -Infinity
			return a - b;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double Add(Double a, Double b) {
			if (b < 0)
				return Subtract(a, -b);

			// Float arithmetic doesn't overflow, but it does follow IEEE 754 rules.  Notably:
			//   NaN       + anything  = NaN
			//   anything  + NaN       = NaN
			//   Infinity  + not NaN   = Infinity
			//   not NaN   + Infinity  = Infinity
			//   -Infinity + not NaN  = -Infinity
			//   not NaN   + -Infinity = -Infinity
			return a + b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double Subtract(Double a, Double b) {
			if (b < 0)
				return Add(a, -b);

			// Float arithmetic doesn't overflow, but it does follow IEEE 754 rules.  Notably:
			//   NaN       - anything  = NaN
			//   anything  - NaN       = NaN
			//   Infinity  - not NaN   = Infinity
			//   not NaN   - Infinity  = Infinity
			//   -Infinity - not NaN  = -Infinity
			//   not NaN   - -Infinity = -Infinity
			return a - b;
		}
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal Add(Decimal a, Decimal b) {
			if (b < 0)
				return Subtract(a, -b);

			// Throws exception on overflow
			try {
				return a + b;
			} catch {
				Registers.F.Overflow = true;
				return a > 0 ? decimal.MaxValue : decimal.MinValue;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal Subtract(Decimal a, Decimal b) {
			if (b < 0 && b != Decimal.MinValue)
				return Add(a, -b);

			// Throws exception on overflow
			try {
				return a - b;
			} catch {
				Registers.F.Overflow = true;
				return a > 0 ? decimal.MaxValue : decimal.MinValue;
			}
		}
		
	}
}