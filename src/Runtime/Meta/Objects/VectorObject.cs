using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _VectorObject : IChipsObject, ICreateObject<_VectorObject>, IConvertToVector<_VectorObject> {
		public _VectorData data;
		public _Vector type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _VectorObject IConvertToVector<_VectorObject>.AsVector(in _VectorObject value, Span<_VectorObject> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _VectorObject ICreateObject<_VectorObject>.Create<U>(in U value) {
			if (typeof(U) == typeof(_VectorObject))
				return Unsafe.As<U, _VectorObject>(ref Unsafe.AsRef(in value));

			_VectorObject obj = default;
			obj.type = TypeMetrics.TypeToVector<U>();
			Unsafe.As<_VectorObject, U>(ref obj) = value;
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong) * 2)]  // Vector<T> has two ulong fields
	internal struct _VectorData {
		[FieldOffset(0)] public Vector2 Vector2;
		[FieldOffset(0)] public Vector3 Vector3;
		[FieldOffset(0)] public Vector4 Vector4;
		// Order of Vector<T> types was taken from Vector<T>::get_IsSupported() source
		[FieldOffset(0)] public Vector<byte> VectorByte;
		[FieldOffset(0)] public Vector<double> VectorDouble;
		[FieldOffset(0)] public Vector<short> VectorInt16;
		[FieldOffset(0)] public Vector<int> VectorInt32;
		[FieldOffset(0)] public Vector<long> VectorInt64;
		[FieldOffset(0)] public Vector<nint> VectorIntPtr;
		[FieldOffset(0)] public Vector<nuint> VectorUIntPtr;
		[FieldOffset(0)] public Vector<sbyte> VectorSByte;
		[FieldOffset(0)] public Vector<float> VectorSingle;
		[FieldOffset(0)] public Vector<ushort> VectorUInt16;
		[FieldOffset(0)] public Vector<uint> VectorUInt32;
		[FieldOffset(0)] public Vector<ulong> VectorUInt64;
	}

	internal enum _Vector {
		Vector2,
		Vector3,
		Vector4,
		VectorT_Byte,
		VectorT_Double,
		VectorT_Int16,
		VectorT_Int32,
		VectorT_Int64,
		VectorT_IntPtr,
		VectorT_UIntPtr,
		VectorT_SByte,
		VectorT_Single,
		VectorT_UInt16,
		VectorT_UInt32,
		VectorT_UInt64
	}
}
