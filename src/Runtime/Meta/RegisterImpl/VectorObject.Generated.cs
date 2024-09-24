using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	partial class ObjectOperations {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_Vector2(in _VectorObject source, in _VectorObject operand) {
			Vector2 result = source.data.Vector2 + operand.data.Vector2;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_Vector3(in _VectorObject source, in _VectorObject operand) {
			Vector3 result = source.data.Vector3 + operand.data.Vector3;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_Vector4(in _VectorObject source, in _VectorObject operand) {
			Vector4 result = source.data.Vector4 + operand.data.Vector4;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorByte(in _VectorObject source, in _VectorObject operand) {
			Vector<byte> result = source.data.VectorByte + operand.data.VectorByte;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorDouble(in _VectorObject source, in _VectorObject operand) {
			Vector<double> result = source.data.VectorDouble + operand.data.VectorDouble;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorInt16(in _VectorObject source, in _VectorObject operand) {
			Vector<short> result = source.data.VectorInt16 + operand.data.VectorInt16;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorInt32(in _VectorObject source, in _VectorObject operand) {
			Vector<int> result = source.data.VectorInt32 + operand.data.VectorInt32;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorInt64(in _VectorObject source, in _VectorObject operand) {
			Vector<long> result = source.data.VectorInt64 + operand.data.VectorInt64;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorSByte(in _VectorObject source, in _VectorObject operand) {
			Vector<sbyte> result = source.data.VectorSByte + operand.data.VectorSByte;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorSingle(in _VectorObject source, in _VectorObject operand) {
			Vector<float> result = source.data.VectorSingle + operand.data.VectorSingle;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorUInt16(in _VectorObject source, in _VectorObject operand) {
			Vector<ushort> result = source.data.VectorUInt16 + operand.data.VectorUInt16;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorUInt32(in _VectorObject source, in _VectorObject operand) {
			Vector<uint> result = source.data.VectorUInt32 + operand.data.VectorUInt32;
			return result.AsVector();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static _VectorObject xAdd_VectorUInt64(in _VectorObject source, in _VectorObject operand) {
			Vector<ulong> result = source.data.VectorUInt64 + operand.data.VectorUInt64;
			return result.AsVector();
		}

	}
}