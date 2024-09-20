using System;
using System.Numerics;
using System.Text;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		// ===== INTEGER VALUES =====

		public void SetAns(byte value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Byte;
			_ans.data.Integer.Byte = value;
		}

		public void SetAns(sbyte value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.SByte;
			_ans.data.Integer.SByte = value;
		}

		public void SetAns(short value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Int16;
			_ans.data.Integer.Int16 = value;
		}

		public void SetAns(ushort value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.UInt16;
			_ans.data.Integer.UInt16 = value;
		}

		public void SetAns(int value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Int32;
			_ans.data.Integer.Int32 = value;
		}

		public void SetAns(uint value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.UInt32;
			_ans.data.Integer.UInt32 = value;
		}

		public void SetAns(long value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Int64;
			_ans.data.Integer.Int64 = value;
		}

		public void SetAns(ulong value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.UInt64;
			_ans.data.Integer.UInt64 = value;
		}

		public void SetAns(char value) {
			_ans.type = _Variant.Integer;
			_ans.subtype = (int)_Integer.Char;
			_ans.data.Integer.Char = value;
		}

		// ===== ADDRESS VALUES =====

		public void SetAns(nint value) {
			_ans.type = _Variant.Address;
			_ans.subtype = (int)_NativeInt.NInt;
			_ans.data.Address.IntPtr = value;
		}

		public void SetAns(nuint value) {
			_ans.type = _Variant.Address;
			_ans.subtype = (int)_NativeInt.NUInt;
			_ans.data.Address.UIntPtr = value;
		}

		// ===== FLOATING-POINT VALUES =====

		public void SetAns(float value) {
			_ans.type = _Variant.Float;
			_ans.subtype = (int)_Float.Single;
			_ans.data.Float.Single = value;
		}

		public void SetAns(double value) {
			_ans.type = _Variant.Float;
			_ans.subtype = (int)_Float.Double;
			_ans.data.Float.Double = value;
		}

		public void SetAns(decimal value) {
			_ans.type = _Variant.Float;
			_ans.subtype = (int)_Float.Decimal;
			_ans.data.Float.Decimal = value;
		}

		// ===== STRING VALUES =====

		public void SetAns(string value) {
			_ans.type = _Variant.String;
			_ans.subtype = (int)_String.String;
			_ans.data.String = value;
		}

		public void SetAns(StringBuilder value) {
			_ans.type = _Variant.String;
			_ans.subtype = (int)_String.StringBuilder;
			_ans.data.String = value;
		}

		// ===== OBJECT VALUES =====

		public void SetAns(object value) {
			_ans.type = _Variant.Object;
			_ans.subtype = 0;
			_ans.data.Object = value;
		}

		// ===== VECTOR VALUES =====

		public void SetAns(Vector2 value) {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.Vector2;
			_ans.data.Vector.Vector2 = value;
		}

		public void SetAns(Vector3 value) {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.Vector3;
			_ans.data.Vector.Vector3 = value;
		}

		public void SetAns(Vector4 value) {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.Vector4;
			_ans.data.Vector.Vector4 = value;
		}

		public void SetAns<T>(Vector<T> value) where T : struct {
			_ans.type = _Variant.Vector;
			_ans.subtype = (int)_Vector.VectorX;
			_VectorData.GetVector<T>(ref _ans.data.Vector) = value;
		}

		// ===== METADATA TOKEN VALUES =====

		public void SetAns(RuntimeTypeHandle value) {
			_ans.type = _Variant.Token;
			_ans.subtype = (int)_MetadataToken.Type;
			_ans.data.Token.TypeHandle = value;
		}

		public void SetAns(RuntimeMethodHandle value) {
			_ans.type = _Variant.Token;
			_ans.subtype = (int)_MetadataToken.Method;
			_ans.data.Token.MethodHandle = value;
		}

		public void SetAns(RuntimeFieldHandle value) {
			_ans.type = _Variant.Token;
			_ans.subtype = (int)_MetadataToken.Field;
			_ans.data.Token.FieldHandle = value;
		}
	}
}
