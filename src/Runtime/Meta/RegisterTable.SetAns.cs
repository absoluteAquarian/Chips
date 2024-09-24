using System;
using System.Numerics;
using System.Text;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		// ===== INTEGER VALUES =====

		public void SetAns(byte value) => _ans = value.AsVariantFromStruct();
		public void SetAns(sbyte value) => _ans = value.AsVariantFromStruct();
		public void SetAns(short value) => _ans = value.AsVariantFromStruct();
		public void SetAns(ushort value) => _ans = value.AsVariantFromStruct();
		public void SetAns(int value) => _ans = value.AsVariantFromStruct();
		public void SetAns(uint value) => _ans = value.AsVariantFromStruct();
		public void SetAns(long value) => _ans = value.AsVariantFromStruct();
		public void SetAns(ulong value) => _ans = value.AsVariantFromStruct();
		public void SetAns(char value) => _ans = value.AsVariantFromStruct();

		// ===== ADDRESS VALUES =====

		public void SetAns(nint value) => _ans = value.AsVariantFromStruct();
		public void SetAns(nuint value) => _ans = value.AsVariantFromStruct();

		// ===== FLOATING-POINT VALUES =====

		public void SetAns(float value) => _ans = value.AsVariantFromStruct();
		public void SetAns(double value) => _ans = value.AsVariantFromStruct();
		public void SetAns(decimal value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Half value) => _ans = value.AsVariantFromStruct();

		// ===== STRING VALUES =====

		public void SetAns(string value) => _ans = value.AsVariantFromObject();
		public void SetAns(StringBuilder value) => _ans = value.AsVariantFromObject();

		// ===== OBJECT VALUES =====

		public void SetAns(object? value) => _ans = value.AsVariantFromObject();
		public void SetAns<T>(in T value) where T : struct => _ans = value.AsVariantFromStruct();

		// ===== VECTOR VALUES =====

		public void SetAns(Vector2 value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector3 value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector4 value) => _ans = value.AsVariantFromStruct();

		public void SetAns(Vector<byte> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<double> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<short> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<int> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<long> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<nint> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<nuint> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<sbyte> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<float> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<ushort> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<uint> value) => _ans = value.AsVariantFromStruct();
		public void SetAns(Vector<ulong> value) => _ans = value.AsVariantFromStruct();

		// ===== METADATA TOKEN VALUES =====

		public void SetAns(RuntimeTypeHandle value) => _ans = value.AsVariantFromStruct();
		public void SetAns(RuntimeMethodHandle value) => _ans = value.AsVariantFromStruct();
		public void SetAns(RuntimeFieldHandle value) => _ans = value.AsVariantFromStruct();

		// ===== EXCEPTION VALUES =====

		public void SetAns(Exception value) => _ans = value.AsVariantFromObject();

		// ===== STATUS VALUES =====

		public void SetAns(Status value) => _ans = value.AsVariantFromStruct();
	}
}
