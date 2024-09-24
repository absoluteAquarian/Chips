using System.Numerics;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private _Address Add_Address_Address(Register source, Register operand) => ObjectOperations.Add_AddressObj_AddressObj(GetAddress(source), GetAddress(operand));

		private _Address Add_Address_Integer(Register source, Register operand) => ObjectOperations.Add_AddressObj_NumberInteger(GetAddress(source), GetInteger(operand));
	}

	partial class ObjectOperations {
		public static _Address Add_AddressObj_AddressObj<TAddressSource, TAddressOperand>(this in TypedRegister<TAddressSource> source, in TypedRegister<TAddressOperand> operand)
		where TAddressSource : struct, IMutableInteger, IConvertToAddress<TAddressSource>
		where TAddressOperand : struct, IMutableInteger, IConvertToAddress<TAddressOperand> {
			ref _Address sourceObj = ref TAddressSource.AsAddress(source.value, stackalloc _Address[1]);
			var operandObj = operand.AsAddress(stackalloc _Address[1]);

			return sourceObj.type switch {
				_NativeInt.NInt => sourceObj.Add_AddressObj_AddressObj_Common<nint>(in operandObj),
				_NativeInt.NUInt => sourceObj.Add_AddressObj_AddressObj_Common<nuint>(in operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _Address Add_AddressObj_AddressObj_Common<TOrig>(this ref _Address source, in TypedRegister<_Address> operand) where TOrig : struct, INumberBase<TOrig> {
			ref var data = ref operand.value.data;

			return operand.value.type switch {
				_NativeInt.NInt => Add_Address_Number_UnknownUpcast<_Address, TOrig, nint>(ref source, data.IntPtr),
				_NativeInt.NUInt => Add_Address_Number_UnknownUpcast<_Address, TOrig, nuint>(ref source, data.UIntPtr),
				_ => throw operand.MalformedArgument()
			};
		}

		public static _Address Add_AddressObj_NumberInteger<TAddress, TInteger>(this in TypedRegister<TAddress> source, in TypedRegister<TInteger> operand)
		where TAddress : struct, IMutableInteger, IConvertToAddress<TAddress>
		where TInteger : struct, IMutableInteger, IConvertToInteger<TInteger> {
			ref _Address sourceObj = ref TAddress.AsAddress(source.value, stackalloc _Address[1]);
			TypedRegister<_NumberInteger> operandObj = operand.AsInteger(stackalloc _NumberInteger[1]);

			return sourceObj.type switch {
				_NativeInt.NInt => sourceObj.Add_AddressObj_NumberInteger_Common<nint>(in operandObj),
				_NativeInt.NUInt => sourceObj.Add_AddressObj_NumberInteger_Common<nuint>(in operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _Address Add_AddressObj_NumberInteger_Common<TOrig>(this ref _Address source, in TypedRegister<_NumberInteger> operand) where TOrig : struct, INumberBase<TOrig>  {
			ref var data = ref operand.value.data;
			return operand.value.type switch {
				_Integer.Int32 => Add_Address_Number_UnknownUpcast<_Address, TOrig, int>(ref source, data.Int32),
				_Integer.Byte => Add_Address_Number_UnknownUpcast<_Address, TOrig, byte>(ref source, data.Byte),
				_Integer.SByte => Add_Address_Number_UnknownUpcast<_Address, TOrig, sbyte>(ref source, data.SByte),
				_Integer.Int16 => Add_Address_Number_UnknownUpcast<_Address, TOrig, short>(ref source, data.Int16),
				_Integer.UInt16 => Add_Address_Number_UnknownUpcast<_Address, TOrig, ushort>(ref source, data.UInt16),
				_Integer.UInt32 => Add_Address_Number_UnknownUpcast<_Address, TOrig, uint>(ref source, data.UInt32),
				_Integer.Int64 => Add_Address_Number_UnknownUpcast<_Address, TOrig, long>(ref source, data.Int64),
				_Integer.UInt64 => Add_Address_Number_UnknownUpcast<_Address, TOrig, ulong>(ref source, data.UInt64),
				_Integer.Char => throw operand.InvalidNumberType<char>(),
				_ => throw operand.MalformedArgument()
			};
		}

		public static string ToString_AddressObj<TAddress>(this in TypedRegister<TAddress> source) where TAddress : struct, IMutableInteger, IConvertToAddress<TAddress> {
			ref _Address sourceObj = ref TAddress.AsAddress(source.value, stackalloc _Address[1]);

			return sourceObj.type switch {
				_NativeInt.NInt => sourceObj.data.IntPtr.ToString(),
				_NativeInt.NUInt => sourceObj.data.UIntPtr.ToString(),
				_ => throw source.MalformedArgument()
			};
		}
	}
}
