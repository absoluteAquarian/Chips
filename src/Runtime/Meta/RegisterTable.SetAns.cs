using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		// ===== INTEGER VALUES =====

		public void SetAns(byte value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Byte;
			Unsafe.As<_VariantObject, byte>(ref _ans) = value;
		}

		public void SetAns(sbyte value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.SByte;
			Unsafe.As<_VariantObject, sbyte>(ref _ans) = value;
		}

		public void SetAns(short value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Int16;
			Unsafe.As<_VariantObject, short>(ref _ans) = value;
		}

		public void SetAns(ushort value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.UInt16;
			Unsafe.As<_VariantObject, ushort>(ref _ans) = value;
		}

		public void SetAns(int value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Int32;
			Unsafe.As<_VariantObject, int>(ref _ans) = value;
		}

		public void SetAns(uint value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.UInt32;
			Unsafe.As<_VariantObject, uint>(ref _ans) = value;
		}

		public void SetAns(long value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Int64;
			Unsafe.As<_VariantObject, long>(ref _ans) = value;
		}

		public void SetAns(ulong value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.UInt64;
			Unsafe.As<_VariantObject, ulong>(ref _ans) = value;
		}

		public void SetAns(char value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Char;
			Unsafe.As<_VariantObject, char>(ref _ans) = value;
		}

		// ===== ADDRESS VALUES =====

		public void SetAns(nint value) {
			_ans.type = _Variant.Address;
			_ans.subtype = (int)_NativeInt.NInt;
			Unsafe.As<_VariantObject, nint>(ref _ans) = value;
		}

		public void SetAns(nuint value) {
			_ans.type = _Variant.Address;
			_ans.subtype = (int)_NativeInt.NUInt;
			Unsafe.As<_VariantObject, nuint>(ref _ans) = value;
		}

		// ===== FLOATING-POINT VALUES =====

		public void SetAns(float value) {
			_ans.type = _Variant.Float;
			_ans.subtype = (int)_Float.Single;
			Unsafe.As<_VariantObject, float>(ref _ans) = value;
		}

		public void SetAns(double value) {
			_ans.type = _Variant.Float;
			_ans.subtype = (int)_Float.Double;
			Unsafe.As<_VariantObject, double>(ref _ans) = value;
		}

		public void SetAns(decimal value) {
			_ans.type = _Variant.Float;
			_ans.subtype = (int)_Float.Decimal;
			Unsafe.As<_VariantObject, decimal>(ref _ans) = value;
		}

		// ===== STRING VALUES =====

		public void SetAns(string value) {
			_ans.type = _Variant.String;
			_ans.subtype = (int)_String.String;
			_VariantObject.GetObject<string>(ref _ans) = value;
		}

		public void SetAns(StringBuilder value) {
			_ans.type = _Variant.String;
			_ans.subtype = (int)_String.StringBuilder;
			_VariantObject.GetObject<StringBuilder>(ref _ans) = value;
		}

		// ===== OBJECT VALUES =====

		public void SetAns(object? value) {
			_ans.type = _Variant.Object;
			_ans.subtype = (int)_VariantObjectSubtype.Reference;
			_VariantObject.GetObject<object>(ref _ans) = value;
		}

		public void SetAns<T>(in T value) where T : struct {
			_ans.type = _Variant.Object;
			_ans.subtype = (int)_VariantObjectSubtype.Valuetype;
			_VariantObject.SetStruct(ref _ans, value);
		}

		// ===== VECTOR VALUES =====

		public void SetAns(Vector2 value) {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.Vector2;
			Unsafe.As<_VariantObject, Vector2>(ref _ans) = value;
		}

		public void SetAns(Vector3 value) {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.Vector3;
			Unsafe.As<_VariantObject, Vector3>(ref _ans) = value;
		}

		public void SetAns(Vector4 value) {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.Vector4;
			Unsafe.As<_VariantObject, Vector4>(ref _ans) = value;
		}

		public void SetAns<T>(in Vector<T> value) where T : struct {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.VectorX;
			Unsafe.As<_VariantObject, Vector<T>>(ref _ans) = value;
		}

		// ===== METADATA TOKEN VALUES =====

		public void SetAns(RuntimeTypeHandle value) {
			_ans.type = _Variant.Token;
			_ans.subtype = (int)_MetadataToken.Type;
			Unsafe.As<_VariantObject, RuntimeTypeHandle>(ref _ans) = value;
		}

		public void SetAns(RuntimeMethodHandle value) {
			_ans.type = _Variant.Token;
			_ans.subtype = (int)_MetadataToken.Method;
			Unsafe.As<_VariantObject, RuntimeMethodHandle>(ref _ans) = value;
		}

		public void SetAns(RuntimeFieldHandle value) {
			_ans.type = _Variant.Token;
			_ans.subtype = (int)_MetadataToken.Field;
			Unsafe.As<_VariantObject, RuntimeFieldHandle>(ref _ans) = value;
		}
	}
}
