using System.Text;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private _StringObject Add_String_String(Register source, Register operand) => ObjectOperations.Add_StringObj_StringObj(GetStringObject(source), GetStringObject(operand));
	}

	partial class ObjectOperations {
		public static _StringObject Add_StringObj_StringObj<TStringSource, TStringOperand>(this in TypedRegister<TStringSource> source, in TypedRegister<TStringOperand> operand)
		where TStringSource : struct, IConvertToString<TStringSource>
		where TStringOperand : struct, IConvertToString<TStringOperand> {
			ref _StringObject sourceObj = ref TStringSource.AsString(source.value, stackalloc _StringObject[1]);
			var operandObj = operand.AsString(stackalloc _StringObject[1]);

			return sourceObj.type switch {
				_String.String => _StringData.AsString(ref sourceObj.data).Add_StringVal_StringObj(operandObj),
				_String.StringBuilder => _StringData.AsStringBuilder(ref sourceObj.data).Add_StringBuilder_StringObj(operandObj),
				_ => throw source.MalformedArgument()
			};
		}

		public static _StringObject Add_StringObj_StringVal<TString>(this in TypedRegister<TString> source, string? operand) where TString : struct, IConvertToString<TString> {
			ref _StringObject sourceObj = ref TString.AsString(source.value, stackalloc _StringObject[1]);

			return sourceObj.type switch {
				_String.String => (_StringData.AsString(ref sourceObj.data) + operand).AsString(),
				_String.StringBuilder => _StringData.AsStringBuilder(ref sourceObj.data).Append(operand).AsString(),
				_ => throw source.MalformedArgument()
			};
		}

		public static _StringObject Add_StringVal_StringObj<TString>(this string? source, in TypedRegister<TString> operand) where TString : struct, IConvertToString<TString> {
			ref _StringObject operandObj = ref TString.AsString(operand.value, stackalloc _StringObject[1]);

			return operandObj.type switch {
				_String.String => (source + _StringData.AsString(ref operandObj.data)).AsString(),
				_String.StringBuilder => (source + _StringData.AsStringBuilder(ref operandObj.data).ToString()).AsString(),
				_ => throw operand.MalformedArgument()
			};
		}

		public static string? Add_StringVal_StringVal(this string? source, string? operand) => source + operand;

		public static _StringObject Add_StringObj_StringBuilder<TString>(this in TypedRegister<TString> source, StringBuilder operand) where TString : struct, IConvertToString<TString> {
			ref _StringObject sourceObj = ref TString.AsString(source.value, stackalloc _StringObject[1]);

			return sourceObj.type switch {
				_String.String => (_StringData.AsString(ref sourceObj.data) + operand.ToString()).AsString(),
				_String.StringBuilder => _StringData.AsStringBuilder(ref sourceObj.data).Append(operand).AsString(),
				_ => throw source.MalformedArgument()
			};
		}

		public static _StringObject Add_StringBuilder_StringObj<TString>(this StringBuilder source, in TypedRegister<TString> operand) where TString : struct, IConvertToString<TString> {
			ref _StringObject operandObj = ref TString.AsString(operand.value, stackalloc _StringObject[1]);

			return operandObj.type switch {
				_String.String => source.Append(_StringData.AsString(ref operandObj.data)).AsString(),
				_String.StringBuilder => source.Append(_StringData.AsStringBuilder(ref operandObj.data)).AsString(),
				_ => throw operand.MalformedArgument()
			};
		}

		public static StringBuilder Add_StringBuilder_StringBuilder(this StringBuilder source, StringBuilder operand) => source.Append(operand);

		public static string? ToString_StringObj<TString>(this in TypedRegister<TString> source) where TString : struct, IConvertToString<TString> {
			ref _StringObject objRef = ref TString.AsString(source.value, stackalloc _StringObject[1]);

			return objRef.type switch {
				_String.String => _StringData.AsString(ref objRef.data),
				_String.StringBuilder => _StringData.AsStringBuilder(ref objRef.data).ToString(),
				_ => throw source.MalformedArgument()
			};
		}
	}
}
