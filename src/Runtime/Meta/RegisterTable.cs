using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	public partial class RegisterTable {
		private struct _Page3<T> {
			public T _0, _1, _2;
		}

		private struct _Page16<T> {
			public T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _A, _B, _C, _D, _E, _F;
		}

		private struct _Page32<T> {
			public _Page16<T> _0, _1;
		}

		private struct _Page64<T> {
			public _Page16<T> _0, _1, _2, _3;
		}

		private readonly BitArray _usedRegisters = new(256);

		private _Page16<_Address> _ptr;
		private _Page32<_NumberInteger> _z;
		private _Page32<_NumberFloat> _fl;
		private _Page16<_StringObject> _s;
		private _Page64<_VariantObject> _obj;
		private _Page16<_VectorObject> _v;
		private _Page16<_MetadataTokenHandle> _t;
		private _Page3<_VariantObject> _r;
		private Exception _ex;
		private Status _ps;
		private _VariantObject _ans;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref T Get<T>(ref _Page3<T> page, int index) {
			switch (index) {
				case 0: return ref page._0;
				case 1: return ref page._1;
				case 2: return ref page._2;
				default: throw new IndexOutOfRangeException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref T Get<T>(ref _Page16<T> page, int index) {
			switch (index) {
				case 0: return ref page._0;
				case 1: return ref page._1;
				case 2: return ref page._2;
				case 3: return ref page._3;
				case 4: return ref page._4;
				case 5: return ref page._5;
				case 6: return ref page._6;
				case 7: return ref page._7;
				case 8: return ref page._8;
				case 9: return ref page._9;
				case 0xA: return ref page._A;
				case 0xB: return ref page._B;
				case 0xC: return ref page._C;
				case 0xD: return ref page._D;
				case 0xE: return ref page._E;
				case 0xF: return ref page._F;
				default: throw new IndexOutOfRangeException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref T Get<T>(ref _Page32<T> page, int index) {
			if (index < 16)
				return ref Get(ref page._0, index);
			else if (index < 32)
				return ref Get(ref page._1, index - 16);
			
			throw new IndexOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref T Get<T>(ref _Page64<T> page, int index) {
			if (index < 16)
				return ref Get(ref page._0, index);
			else if (index < 32)
				return ref Get(ref page._1, index - 16);
			else if (index < 48)
				return ref Get(ref page._2, index - 32);
			else if (index < 64)
				return ref Get(ref page._3, index - 48);
			
			throw new IndexOutOfRangeException();
		}
	}
}
