using System.Numerics;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private _NumberInteger Add_Integer_Integer(Register source, Register operand) => ObjectOperations.Add_NumberInteger_NumberInteger(GetInteger(source), GetInteger(operand));
	}

	partial class ObjectOperations {
		public static _NumberInteger Add_NumberInteger_NumberInteger<TIntegerSource, TIntegerOperand>(this in TypedRegister<TIntegerSource> source, in TypedRegister<TIntegerOperand> operand)
		where TIntegerSource : struct, IConvertToInteger<TIntegerSource>
		where TIntegerOperand : struct, IConvertToInteger<TIntegerOperand> {
			ref _NumberInteger sourceObj = ref TIntegerSource.AsInteger(in source.value, stackalloc _NumberInteger[1]);
			var operandObj = operand.AsInteger(stackalloc _NumberInteger[1]);

			return sourceObj.type switch {
				_Integer.Int32 => sourceObj.Add_NumberInteger_NumberInteger_Common<int>(in operandObj),
				_Integer.Byte => sourceObj.Add_NumberInteger_NumberInteger_Common<byte>(in operandObj),
				_Integer.SByte => sourceObj.Add_NumberInteger_NumberInteger_Common<sbyte>(in operandObj),
				_Integer.Int16 => sourceObj.Add_NumberInteger_NumberInteger_Common<short>(in operandObj),
				_Integer.UInt16 => sourceObj.Add_NumberInteger_NumberInteger_Common<ushort>(in operandObj),
				_Integer.UInt32 => sourceObj.Add_NumberInteger_NumberInteger_Common<uint>(in operandObj),
				_Integer.Int64 => sourceObj.Add_NumberInteger_NumberInteger_Common<long>(in operandObj),
				_Integer.UInt64 => sourceObj.Add_NumberInteger_NumberInteger_Common<ulong>(in operandObj),
				_Integer.Char => sourceObj.Add_NumberInteger_NumberInteger_Common<char>(in operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _NumberInteger Add_NumberInteger_NumberInteger_Common<TOrig>(this ref _NumberInteger source, in TypedRegister<_NumberInteger> operand) where TOrig : struct, INumberBase<TOrig> {
			ref var data = ref operand.value.data;

			return operand.value.type switch {
				_Integer.Int32 => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, int>(data.Int32),
				_Integer.Byte => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, byte>(data.Byte),
				_Integer.SByte => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, sbyte>(data.SByte),
				_Integer.Int16 => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, short>(data.Int16),
				_Integer.UInt16 => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, ushort>(data.UInt16),
				_Integer.UInt32 => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, uint>(data.UInt32),
				_Integer.Int64 => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, long>(data.Int64),
				_Integer.UInt64 => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, ulong>(data.UInt64),
				_Integer.Char => source.Add_Int_Number_UnknownUpcast<_NumberInteger, TOrig, char>(data.Char),
				_ => throw operand.MalformedArgument()
			};
		}

		public static string ToString_NumberInteger<TInteger>(this in TypedRegister<TInteger> source) where TInteger : struct, IMutableInteger, IConvertToInteger<TInteger> {
			ref _NumberInteger obj = ref TInteger.AsInteger(in source.value, stackalloc _NumberInteger[1]);

			return obj.type switch {
				_Integer.Int32 => obj.data.Int32.ToString(),
				_Integer.Byte => obj.data.Byte.ToString(),
				_Integer.SByte => obj.data.SByte.ToString(),
				_Integer.Int16 => obj.data.Int16.ToString(),
				_Integer.UInt16 => obj.data.UInt16.ToString(),
				_Integer.UInt32 => obj.data.UInt32.ToString(),
				_Integer.Int64 => obj.data.Int64.ToString(),
				_Integer.UInt64 => obj.data.UInt64.ToString(),
				_Integer.Char => obj.data.Char.ToString(),
				_ => throw source.MalformedArgument()
			};
		}
	}
}
