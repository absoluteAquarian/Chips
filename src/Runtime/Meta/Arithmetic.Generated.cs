using System;

namespace Chips.Runtime.Meta {
	public static partial class Arithmetic {
		public static void RotateLeft(ref Status ps, SByte value, int bits, ref SByte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(SByte) * 8 - ONE_SMALL))) != 0;
				value = (SByte)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(SByte value, int bits, ref SByte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & (ONE << (sizeof(SByte) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (SByte)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, SByte value, int bits, ref SByte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? (ONE << (sizeof(SByte) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (SByte)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(SByte value, int bits, ref SByte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(SByte) * 8 - ONE_SMALL)) : 0;
				value = (SByte)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, Int16 value, int bits, ref Int16 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(Int16) * 8 - ONE_SMALL))) != 0;
				value = (Int16)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(Int16 value, int bits, ref Int16 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & (ONE << (sizeof(Int16) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (Int16)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, Int16 value, int bits, ref Int16 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? (ONE << (sizeof(Int16) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (Int16)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(Int16 value, int bits, ref Int16 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(Int16) * 8 - ONE_SMALL)) : 0;
				value = (Int16)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, Int32 value, int bits, ref Int32 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(Int32) * 8 - ONE_SMALL))) != 0;
				value = (Int32)((Int32)((Int32)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(Int32 value, int bits, ref Int32 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & (ONE << (sizeof(Int32) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (Int32)((Int32)((Int32)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, Int32 value, int bits, ref Int32 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? (ONE << (sizeof(Int32) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (Int32)((Int32)((Int32)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(Int32 value, int bits, ref Int32 target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(Int32) * 8 - ONE_SMALL)) : 0;
				value = (Int32)((Int32)((Int32)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, Int64 value, int bits, ref Int64 target) {
			const Int64 ONE = 1L;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int64 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(Int64) * 8 - ONE_SMALL))) != 0;
				value = (Int64)((Int64)((Int64)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(Int64 value, int bits, ref Int64 target) {
			const Int64 ONE = 1L;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int64 shiftIn = (value & (ONE << (sizeof(Int64) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (Int64)((Int64)((Int64)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, Int64 value, int bits, ref Int64 target) {
			const Int64 ONE = 1L;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int64 shiftIn = carry ? (ONE << (sizeof(Int64) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (Int64)((Int64)((Int64)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(Int64 value, int bits, ref Int64 target) {
			const Int64 ONE = 1L;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int64 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(Int64) * 8 - ONE_SMALL)) : 0;
				value = (Int64)((Int64)((Int64)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, Byte value, int bits, ref Byte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(Byte) * 8 - ONE_SMALL))) != 0;
				value = (Byte)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(Byte value, int bits, ref Byte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & (ONE << (sizeof(Byte) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (Byte)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, Byte value, int bits, ref Byte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = carry ? (ONE << (sizeof(Byte) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (Byte)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(Byte value, int bits, ref Byte target) {
			const Int32 ONE = 1;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				Int32 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(Byte) * 8 - ONE_SMALL)) : 0;
				value = (Byte)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, UInt16 value, int bits, ref UInt16 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(UInt16) * 8 - ONE_SMALL))) != 0;
				value = (UInt16)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(UInt16 value, int bits, ref UInt16 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = (value & (ONE << (sizeof(UInt16) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (UInt16)((UInt16)((UInt16)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, UInt16 value, int bits, ref UInt16 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = carry ? (ONE << (sizeof(UInt16) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (UInt16)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(UInt16 value, int bits, ref UInt16 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(UInt16) * 8 - ONE_SMALL)) : 0;
				value = (UInt16)((UInt16)((UInt16)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, UInt32 value, int bits, ref UInt32 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(UInt32) * 8 - ONE_SMALL))) != 0;
				value = (UInt32)((UInt32)((UInt32)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(UInt32 value, int bits, ref UInt32 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = (value & (ONE << (sizeof(UInt32) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (UInt32)((UInt32)((UInt32)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, UInt32 value, int bits, ref UInt32 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = carry ? (ONE << (sizeof(UInt32) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (UInt32)((UInt32)((UInt32)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(UInt32 value, int bits, ref UInt32 target) {
			const UInt32 ONE = 1u;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				UInt32 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(UInt32) * 8 - ONE_SMALL)) : 0;
				value = (UInt32)((UInt32)((UInt32)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateLeft(ref Status ps, UInt64 value, int bits, ref UInt64 target) {
			const UInt64 ONE = 1uL;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				UInt64 shiftIn = carry ? ONE : 0;
				carry = (value & (ONE << (sizeof(UInt64) * 8 - ONE_SMALL))) != 0;
				value = (UInt64)((UInt64)((UInt64)value << ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateLeftNoCarry(UInt64 value, int bits, ref UInt64 target) {
			const UInt64 ONE = 1uL;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				UInt64 shiftIn = (value & (ONE << (sizeof(UInt64) * 8 - ONE_SMALL))) != 0 ? ONE : 0; 
				value = (UInt64)((UInt64)((UInt64)value << ONE_SMALL) | shiftIn);
			}

			target = value;
		}

		public static void RotateRight(ref Status ps, UInt64 value, int bits, ref UInt64 target) {
			const UInt64 ONE = 1uL;
			const int ONE_SMALL = 1;
			bool carry = ps.Carry;

			for (int i = 0; i < bits; i++) {
				UInt64 shiftIn = carry ? (ONE << (sizeof(UInt64) * 8 - ONE_SMALL)) : 0;
				carry = (value & ONE) != 0;
				value = (UInt64)((UInt64)((UInt64)value >> ONE_SMALL) | shiftIn);
			}

			ps.Update(value);
			ps.Carry = carry;
			target = value;
		}

		public static void RotateRight(UInt64 value, int bits, ref UInt64 target) {
			const UInt64 ONE = 1uL;
			const int ONE_SMALL = 1;

			for (int i = 0; i < bits; i++) {
				UInt64 shiftIn = (value & ONE) != 0 ? (ONE << (sizeof(UInt64) * 8 - ONE_SMALL)) : 0;
				value = (UInt64)((UInt64)((UInt64)value >> ONE_SMALL) | shiftIn);
			}

			target = value;
		}

	}
}