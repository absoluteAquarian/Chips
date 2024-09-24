using System;
using System.Numerics;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private _VariantObject Add_Variant_Variant(Register source, Register operand) => ObjectOperations.Add_VariantObj_VariantObj(GetVariantObject(source), GetVariantObject(operand));
	}

	partial class ObjectOperations {
		public static _VariantObject Add_VariantObj_VariantObj(this in TypedRegister<_VariantObject> source, in TypedRegister<_VariantObject> operand) {
			ref var data = ref operand.value.data;

			return operand.value.type switch {
				_Variant.Integer => (_Integer)operand.value.subtype switch {
					_Integer.Int32 => source.Add_VariantObj_Number(data.Integer.Int32),
					_Integer.Byte => source.Add_VariantObj_Number(data.Integer.Byte),
					_Integer.SByte => source.Add_VariantObj_Number(data.Integer.SByte),
					_Integer.Int16 => source.Add_VariantObj_Number(data.Integer.Int16),
					_Integer.UInt16 => source.Add_VariantObj_Number(data.Integer.UInt16),
					_Integer.UInt32 => source.Add_VariantObj_Number(data.Integer.UInt32),
					_Integer.Int64 => source.Add_VariantObj_Number(data.Integer.Int64),
					_Integer.UInt64 => source.Add_VariantObj_Number(data.Integer.UInt64),
					_Integer.Char => source.Add_VariantObj_Number(data.Integer.Char),
					_ => throw operand.MalformedArgument()
				},
				_Variant.Address => (_NativeInt)operand.value.type switch {
					_NativeInt.NInt => source.Add_VariantObj_Number(data.Address.IntPtr),
					_NativeInt.NUInt => source.Add_VariantObj_Number(data.Address.UIntPtr),
					_ => throw operand.MalformedArgument()
				},
				_Variant.Float => (_Float)operand.value.type switch {
					_Float.Single => source.Add_VariantObj_Number(data.Float.Single),
					_Float.Double => source.Add_VariantObj_Number(data.Float.Double),
					_Float.Decimal => source.Add_VariantObj_Number(data.Float.Decimal),
					_Float.Half => source.Add_VariantObj_Number(data.Float.Half),
					_ => throw operand.MalformedArgument()
				},
				_Variant.String => source.Add_VariantObj_StringObj(operand),
				_Variant.Object => throw new NotImplementedException("Implementation for variant Object not found"),  // TODO: Implement
				_Variant.Vector => source.Add_VariantObj_VectorObj(operand),
				_Variant.Exception => source.Add_VariantObj_ExceptionObj(operand),
				_ => throw source.InvalidType()
			};
		}

		public static _VariantObject Add_VariantObj_NumberInteger<TInteger>(this in TypedRegister<_VariantObject> source, in TypedRegister<TInteger> operand) where TInteger : struct, IConvertToInteger<TInteger> {
			ref _NumberInteger operandObj = ref TInteger.AsInteger(in operand.value, stackalloc _NumberInteger[1]);
			ref var data = ref operandObj.data;

			return operandObj.type switch {
				_Integer.Int32 => source.Add_VariantObj_Number(data.Int32),
				_Integer.Byte => source.Add_VariantObj_Number(data.Byte),
				_Integer.SByte => source.Add_VariantObj_Number(data.SByte),
				_Integer.Int16 => source.Add_VariantObj_Number(data.Int16),
				_Integer.UInt16 => source.Add_VariantObj_Number(data.UInt16),
				_Integer.UInt32 => source.Add_VariantObj_Number(data.UInt32),
				_Integer.Int64 => source.Add_VariantObj_Number(data.Int64),
				_Integer.UInt64 => source.Add_VariantObj_Number(data.UInt64),
				_Integer.Char => source.Add_VariantObj_Number(data.Char),
				_ => throw operand.MalformedArgument()
			};
		}

		public static _VariantObject Add_VariantObj_Address<TAddress>(this in TypedRegister<_VariantObject> source, in TypedRegister<TAddress> operand) where TAddress : struct, IConvertToAddress<TAddress> {
			ref _Address operandObj = ref TAddress.AsAddress(in operand.value, stackalloc _Address[1]);
			ref var data = ref operandObj.data;

			return operandObj.type switch {
				_NativeInt.NInt => source.Add_VariantObj_Number(data.IntPtr),
				_NativeInt.NUInt => source.Add_VariantObj_Number(data.UIntPtr),
				_ => throw operand.MalformedArgument()
			};
		}

		public static _VariantObject Add_VariantObj_NumberFloat<TFloat>(this in TypedRegister<_VariantObject> source, in TypedRegister<TFloat> operand) where TFloat : struct, IConvertToFloat<TFloat> {
			ref _NumberFloat operandObj = ref TFloat.AsFloat(in operand.value, stackalloc _NumberFloat[1]);
			ref var data = ref operandObj.data;

			return operandObj.type switch {
				_Float.Single => source.Add_VariantObj_Number(data.Single),
				_Float.Double => source.Add_VariantObj_Number(data.Double),
				_Float.Decimal => source.Add_VariantObj_Number(data.Decimal),
				_Float.Half => source.Add_VariantObj_Number(data.Half),
				_ => throw operand.MalformedArgument()
			};
		}

		public static _VariantObject Add_VariantObj_Number<T>(this in TypedRegister<_VariantObject> source, T operand) where T : struct, INumberBase<T> {
			return source.value.type switch {
				_Variant.Integer => (_Integer)source.value.subtype switch {
					_Integer.Int32 => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, int, T>(operand),
					_Integer.Byte => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, byte, T>(operand),
					_Integer.SByte => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, sbyte, T>(operand),
					_Integer.Int16 => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, short, T>(operand),
					_Integer.UInt16 => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, ushort, T>(operand),
					_Integer.UInt32 => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, uint, T>(operand),
					_Integer.Int64 => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, long, T>(operand),
					_Integer.UInt64 => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, ulong, T>(operand),
					_Integer.Char => source.value.Add_Int_Number_UnknownUpcast<_VariantObject, char, T>(operand),
					_ => throw source.MalformedArgument()
				},
				_Variant.Address => (_NativeInt)source.value.subtype switch {
					_NativeInt.NInt => source.value.Add_Address_Number_UnknownUpcast<_VariantObject, nint, T>(operand),
					_NativeInt.NUInt => source.value.Add_Address_Number_UnknownUpcast<_VariantObject, nuint, T>(operand),
					_ => throw source.MalformedArgument()
				},
				_Variant.Float => (_Float)source.value.subtype switch {
					_Float.Single => source.value.Add_Float_Number_UnknownUpcast<_VariantObject, float, T>(operand),
					_Float.Double => source.value.Add_Float_Number_UnknownUpcast<_VariantObject, double, T>(operand),
					_Float.Decimal => source.value.Add_Float_Number_UnknownUpcast<_VariantObject, decimal, T>(operand),
					_Float.Half => source.value.Add_Float_Number_UnknownUpcast<_VariantObject, Half, T>(operand),
					_ => throw source.MalformedArgument()
				},
				_Variant.String => source.Add_StringObj_StringVal(operand.ToString()).AsVariant(),
				_Variant.Object => throw new NotImplementedException("Implementation for variant Object not found"),  // TODO: Implement
				_ => throw source.InvalidType()
			};
		}

		public static _VariantObject Add_VariantObj_StringObj<TString>(this in TypedRegister<_VariantObject> source, in TypedRegister<TString> operand) where TString : struct, IConvertToString<TString> {
			return source.value.type switch {
				_Variant.Integer or _Variant.Address or _Variant.Float or _Variant.Vector or _Variant.Token => source.ToString_VariantObj().Add_StringVal_StringObj(operand).AsVariant(),
				_Variant.String => source.Add_StringObj_StringObj(operand).AsVariant(),
				_Variant.Object => throw new NotImplementedException("Implementation for variant Object not found"),  // TODO: Implement
				_Variant.Exception => source.ToString_Exception().Add_StringVal_StringObj(operand).AsVariant(),
				_ => throw source.InvalidType()
			};
		}

		public static _VariantObject Add_StringObj_VariantObj<TString>(this in TypedRegister<TString> source, in TypedRegister<_VariantObject> operand) where TString : struct, IConvertToString<TString> {
			return operand.value.type switch {
				_Variant.Integer or _Variant.Address or _Variant.Float or _Variant.Vector or _Variant.Token => source.Add_StringObj_StringVal(ToString_VariantObj(operand)).AsVariant(),
				_Variant.String => source.Add_StringObj_StringObj(operand).AsVariant(),
				_Variant.Object => throw new NotImplementedException("Implementation for variant Object not found"),  // TODO: Implement
				_ => throw operand.InvalidType()
			};
		}

		public static _VariantObject Add_VariantObj_VectorObj<TVector>(this in TypedRegister<_VariantObject> source, in TypedRegister<TVector> operand) where TVector : struct, IConvertToVector<TVector> {
			return source.value.type switch {
				_Variant.String => source.ToString_VariantObj().Add_StringVal_StringVal(operand.ToString_VectorObj()).AsVariant(),
				_Variant.Vector => source.Add_VectorObj_VectorObj(operand).AsVariant(),
				_ => throw source.InvalidType()
			};
		}

		public static _VariantObject Add_VectorObj_VariantObj(this in TypedRegister<_VectorObject> source, in TypedRegister<_VariantObject> operand) {
			return operand.value.type switch {
				_Variant.String => source.ToString_VectorObj().Add_StringVal_StringObj(operand).AsVariant(),
				_Variant.Vector => Add_VariantObj_VectorObj(operand, source),
				_ => throw operand.InvalidType()
			};
		}

		public static _VariantObject Add_VariantObj_ExceptionObj<TException>(this in TypedRegister<_VariantObject> source, in TypedRegister<TException> operand) where TException : struct, IConvertToException<TException> {
			return source.value.type switch {
				_Variant.String => source.Add_StringObj_StringVal(operand.ToString_Exception()).AsVariant(),
				_ => throw source.InvalidType()
			};
		}

		public static _VariantObject Add_ExceptionObj_VariantObj<TException>(this in TypedRegister<TException> source, in TypedRegister<_VariantObject> operand) where TException : struct, IConvertToException<TException> {
			return operand.value.type switch {
				_Variant.String => source.ToString_Exception().Add_StringVal_StringObj(operand).AsVariant(),
				_ => throw operand.InvalidType()
			};
		}

		public static string? ToString_VariantObj(this in TypedRegister<_VariantObject> source) {
			return source.value.type switch {
				_Variant.Integer => source.ToString_NumberInteger(),
				_Variant.Address => source.ToString_AddressObj(),
				_Variant.Float => source.ToString_NumberFloat(),
				_Variant.String => source.ToString_StringObj(),
				_Variant.Object => source.ToString_Object(),
				_Variant.Vector => source.ToString_VectorObj(),
				_Variant.Token => source.ToString_Token(),
				_Variant.Exception => source.ToString_Exception(),
				_Variant.Status => throw new NotImplementedException("Status object does not have a string representation"),
				_ => throw source.InvalidType()
			};
		}
	}
}