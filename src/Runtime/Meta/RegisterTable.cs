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
		private _Page64<_Object> _obj;
		private _Page16<_VectorObject> _v;
		private _Page16<_TokenHandle> _t;
		private _Page3<_VariantObject> _r;
		private _ExceptionObject _ex;
		private _StatusObject _ps;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressRegister(Register register) => register >= Register.PTR_0 && register <= Register.PTR_F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_Address> GetAddress(Register register) {
			if (!IsAddressRegister(register))
				throw new NotAnAddressRegisterException(register);

			return new TypedRegister<_Address>(register, ref Get(ref _ptr, register - Register.PTR_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsIntegerRegister(Register register) => register >= Register.Z_0 && register <= Register.Z_1F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_NumberInteger> GetInteger(Register register) {
			if (!IsIntegerRegister(register))
				throw new NotAnIntegerRegisterException(register);

			return new TypedRegister<_NumberInteger>(register, ref Get(ref _z, register - Register.Z_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFloatRegister(Register register) => register >= Register.FL_0 && register <= Register.FL_1F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_NumberFloat> GetFloat(Register register) {
			if (!IsFloatRegister(register))
				throw new NotAFloatRegisterException(register);

			return new TypedRegister<_NumberFloat>(register, ref Get(ref _fl, register - Register.FL_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsStringRegister(Register register) => register >= Register.S_0 && register <= Register.S_F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_StringObject> GetStringObject(Register register) {
			if (!IsStringRegister(register))
				throw new NotAStringRegisterException(register);

			return new TypedRegister<_StringObject>(register, ref Get(ref _s, register - Register.S_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsObjectRegister(Register register) => register >= Register.OBJ_0 && register <= Register.OBJ_3F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_Object> GetObject(Register register) {
			if (!IsObjectRegister(register))
				throw new NotAnObjectRegisterException(register);

			return new TypedRegister<_Object>(register, ref Get(ref _obj, register - Register.OBJ_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsVectorRegister(Register register) => register >= Register.V_0 && register <= Register.V_F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_VectorObject> GetVectorObject(Register register) {
			if (!IsVectorRegister(register))
				throw new NotAVectorRegisterException(register);

			return new TypedRegister<_VectorObject>(register, ref Get(ref _v, register - Register.V_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMetadataTokenRegister(Register register) => register >= Register.T_0 && register <= Register.T_F;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_TokenHandle> GetToken(Register register) {
			if (!IsMetadataTokenRegister(register))
				throw new NotATokenRegisterException(register);

			return new TypedRegister<_TokenHandle>(register, ref Get(ref _t, register - Register.T_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsVariantObjectRegister(Register register) => (register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_VariantObject> GetVariantObject(Register register) {
			if (!IsVariantObjectRegister(register))
				throw new NotAVariantRegisterException(register);

			return new TypedRegister<_VariantObject>(register, ref Get(ref _r, register - Register.R_0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsExceptionRegister(Register register) => register == Register.EX;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_ExceptionObject> GetException(Register register) {
			if (!IsExceptionRegister(register))
				throw new NotAnExceptionRegisterException(register);

			return new TypedRegister<_ExceptionObject>(register, ref _ex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TypedRegister<_StatusObject> GetStatus() => new TypedRegister<_StatusObject>(Register.PS, ref _ps);
	}
}
