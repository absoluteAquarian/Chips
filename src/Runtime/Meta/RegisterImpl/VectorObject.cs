using System.Numerics;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private _VectorObject Add_Vector_Vector(Register source, Register operand) => ObjectOperations.Add_VectorObj_VectorObj(GetVectorObject(source), GetVectorObject(operand));
	}

	partial class ObjectOperations {
		public static _VectorObject Add_VectorObj_VectorObj<TVectorSource, TVectorOperand>(this in TypedRegister<TVectorSource> source, in TypedRegister<TVectorOperand> operand)
		where TVectorSource : struct, IConvertToVector<TVectorSource>
		where TVectorOperand : struct, IConvertToVector<TVectorOperand> {
			ref _VectorObject sourceObj = ref TVectorSource.AsVector(source.value, stackalloc _VectorObject[1]);
			var operandObj = operand.AsVector(stackalloc _VectorObject[1]);

			return sourceObj.type switch {
				_Vector.Vector2 => sourceObj.Add_VectorObj_VectorObj_Common<Vector2>(operandObj),
				_Vector.Vector3 => sourceObj.Add_VectorObj_VectorObj_Common<Vector3>(operandObj),
				_Vector.Vector4 => sourceObj.Add_VectorObj_VectorObj_Common<Vector4>(operandObj),
				_Vector.VectorT_Byte => sourceObj.Add_VectorObj_VectorObj_Common<Vector<byte>>(operandObj),
				_Vector.VectorT_Double => sourceObj.Add_VectorObj_VectorObj_Common<Vector<double>>(operandObj),
				_Vector.VectorT_Int16 => sourceObj.Add_VectorObj_VectorObj_Common<Vector<short>>(operandObj),
				_Vector.VectorT_Int32 => sourceObj.Add_VectorObj_VectorObj_Common<Vector<int>>(operandObj),
				_Vector.VectorT_Int64 => sourceObj.Add_VectorObj_VectorObj_Common<Vector<long>>(operandObj),
				_Vector.VectorT_IntPtr => sourceObj.Add_VectorObj_VectorObj_Common<Vector<nint>>(operandObj),
				_Vector.VectorT_UIntPtr => sourceObj.Add_VectorObj_VectorObj_Common<Vector<nuint>>(operandObj),
				_Vector.VectorT_SByte => sourceObj.Add_VectorObj_VectorObj_Common<Vector<sbyte>>(operandObj),
				_Vector.VectorT_Single => sourceObj.Add_VectorObj_VectorObj_Common<Vector<float>>(operandObj),
				_Vector.VectorT_UInt16 => sourceObj.Add_VectorObj_VectorObj_Common<Vector<ushort>>(operandObj),
				_Vector.VectorT_UInt32 => sourceObj.Add_VectorObj_VectorObj_Common<Vector<uint>>(operandObj),
				_Vector.VectorT_UInt64 => sourceObj.Add_VectorObj_VectorObj_Common<Vector<ulong>>(operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _VectorObject Add_VectorObj_VectorObj_Common<TExpected>(this ref _VectorObject source, in TypedRegister<_VectorObject> operand) {
			if (operand.value.type != TypeMetrics.TypeToVector<TExpected>())
				throw new InvalidVectorTypeException<TExpected>();

			return operand.value.type switch {
				_Vector.Vector2 => Add_Vector2(source, operand.value),
				_Vector.Vector3 => Add_Vector3(source, operand.value),
				_Vector.Vector4 => Add_Vector4(source, operand.value),
				_Vector.VectorT_Byte => Add_VectorByte(source, operand.value),
				_Vector.VectorT_Double => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_Int16 => Add_VectorInt16(source, operand.value),
				_Vector.VectorT_Int32 => Add_VectorInt32(source, operand.value),
				_Vector.VectorT_Int64 => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_IntPtr => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_UIntPtr => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_SByte => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_Single => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_UInt16 => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_UInt32 => Add_VectorDouble(source, operand.value),
				_Vector.VectorT_UInt64 => Add_VectorDouble(source, operand.value),
				_ => throw operand.MalformedArgument()
			};
		}

		public static string ToString_VectorObj<TVector>(this in TypedRegister<TVector> source) where TVector : struct, IConvertToVector<TVector> {
			ref _VectorObject sourceObj = ref TVector.AsVector(source.value, stackalloc _VectorObject[1]);

			return sourceObj.type switch {
				_Vector.Vector2 => sourceObj.data.Vector2.ToString(),
				_Vector.Vector3 => sourceObj.data.Vector3.ToString(),
				_Vector.Vector4 => sourceObj.data.Vector4.ToString(),
				_Vector.VectorT_Byte => sourceObj.data.VectorByte.ToString(),
				_Vector.VectorT_Double => sourceObj.data.VectorDouble.ToString(),
				_Vector.VectorT_Int16 => sourceObj.data.VectorInt16.ToString(),
				_Vector.VectorT_Int32 => sourceObj.data.VectorInt32.ToString(),
				_Vector.VectorT_Int64 => sourceObj.data.VectorInt64.ToString(),
				_Vector.VectorT_IntPtr => sourceObj.data.VectorIntPtr.ToString(),
				_Vector.VectorT_UIntPtr => sourceObj.data.VectorUIntPtr.ToString(),
				_Vector.VectorT_SByte => sourceObj.data.VectorSByte.ToString(),
				_Vector.VectorT_Single => sourceObj.data.VectorSingle.ToString(),
				_Vector.VectorT_UInt16 => sourceObj.data.VectorUInt16.ToString(),
				_Vector.VectorT_UInt32 => sourceObj.data.VectorUInt32.ToString(),
				_Vector.VectorT_UInt64 => sourceObj.data.VectorUInt64.ToString(),
				_ => throw new InvalidObjectStateException(nameof(source))
			};
		}
	}
}
