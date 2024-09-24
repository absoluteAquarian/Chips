using System;
using System.Numerics;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private _NumberFloat Add_Float_Float(Register source, Register operand) => ObjectOperations.Add_NumberFloat_NumberFloat(GetFloat(source), GetFloat(operand));

		private _NumberFloat Add_Float_Integer(Register source, Register operand) => ObjectOperations.Add_NumberFloat_NumberInteger(GetFloat(source), GetInteger(operand));
	}

	partial class ObjectOperations {
		public static _NumberFloat Add_NumberFloat_NumberFloat<TFloatSource, TFloatOperand>(this in TypedRegister<TFloatSource> source, in TypedRegister<TFloatOperand> operand)
		where TFloatSource : struct, IConvertToFloat<TFloatSource>
		where TFloatOperand : struct, IConvertToFloat<TFloatOperand> {
			ref _NumberFloat sourceObj = ref TFloatSource.AsFloat(in source.value, stackalloc _NumberFloat[1]);
			var operandObj = operand.AsFloat(stackalloc _NumberFloat[1]);

			return sourceObj.type switch {
				_Float.Single => sourceObj.Add_NumberFloat_NumberFloat_Common<float>(in operandObj),
				_Float.Double => sourceObj.Add_NumberFloat_NumberFloat_Common<double>(in operandObj),
				_Float.Decimal => sourceObj.Add_NumberFloat_NumberFloat_Common<decimal>(in operandObj),
				_Float.Half => sourceObj.Add_NumberFloat_NumberFloat_Common<Half>(in operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _NumberFloat Add_NumberFloat_NumberFloat_Common<TOrig>(this ref _NumberFloat source, in TypedRegister<_NumberFloat> operand) where TOrig : struct, INumberBase<TOrig> {
			ref var data = ref operand.value.data;

			return operand.value.type switch {
				_Float.Single => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, float>(data.Single),
				_Float.Double => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, double>(data.Double),
				_Float.Decimal => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, decimal>(data.Decimal),
				_Float.Half => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, Half>(data.Half),
				_ => throw operand.MalformedArgument()
			};
		}

		public static _NumberFloat Add_NumberFloat_NumberInteger<TFloat, TInteger>(this in TypedRegister<TFloat> source, in TypedRegister<TInteger> operand)
		where TFloat : struct, IConvertToFloat<TFloat>
		where TInteger : struct, IConvertToInteger<TInteger> {
			ref _NumberFloat sourceObj = ref TFloat.AsFloat(in source.value, stackalloc _NumberFloat[1]);
			var operandObj = operand.AsInteger(stackalloc _NumberInteger[1]);

			return sourceObj.type switch {
				_Float.Single => sourceObj.Add_NumberFloat_NumberInteger_Common<float>(in operandObj),
				_Float.Double => sourceObj.Add_NumberFloat_NumberInteger_Common<double>(in operandObj),
				_Float.Decimal => sourceObj.Add_NumberFloat_NumberInteger_Common<decimal>(in operandObj),
				_Float.Half => sourceObj.Add_NumberFloat_NumberInteger_Common<Half>(in operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _NumberFloat Add_NumberFloat_NumberInteger_Common<TOrig>(this ref _NumberFloat source, in TypedRegister<_NumberInteger> operand) where TOrig : struct, INumberBase<TOrig> {
			ref var data = ref operand.value.data;

			return operand.value.type switch {
				_Integer.Int32 => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, int>(data.Int32),
				_Integer.Byte => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, byte>(data.Byte),
				_Integer.SByte => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, sbyte>(data.SByte),
				_Integer.Int16 => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, short>(data.Int16),
				_Integer.UInt16 => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, ushort>(data.UInt16),
				_Integer.UInt32 => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, uint>(data.UInt32),
				_Integer.Int64 => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, long>(data.Int64),
				_Integer.UInt64 => source.Add_Float_Number_UnknownUpcast<_NumberFloat, TOrig, ulong>(data.UInt64),
				_Integer.Char => throw operand.InvalidNumberType<char>(),
				_ => throw operand.MalformedArgument()
			};
		}

		public static string ToString_NumberFloat<TFloat>(this in TypedRegister<TFloat> source) where TFloat : struct, IConvertToFloat<TFloat> {
			ref _NumberFloat sourceObj = ref TFloat.AsFloat(in source.value, stackalloc _NumberFloat[1]);

			return sourceObj.type switch {
				_Float.Single => sourceObj.data.Single.ToString(),
				_Float.Double => sourceObj.data.Double.ToString(),
				_Float.Decimal => sourceObj.data.Decimal.ToString(),
				_Float.Half => sourceObj.data.Half.ToString(),
				_ => throw source.MalformedArgument()
			};
		}
	}
}
