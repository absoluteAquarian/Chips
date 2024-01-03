using Chips.Runtime.Types.NumberProcessing;
using System.Collections.Generic;

namespace Chips.Runtime.Utility {
	public static class TypeTracking {
		internal delegate bool ParseValue(string? str, out object? result);
		private delegate bool ParseValue<T>(string? str, out T result);

		private static ParseValue CreateParseValueDelegate<T>(ParseValue<T> func)
			=> (string? str, out object? result) => {
				bool success = func(str, out T typedResult);
				result = typedResult;
				return success;
			};

		internal static readonly Dictionary<string, ParseValue> cachedParseFuncs = new() {
			["int"] = CreateParseValueDelegate<int>(int.TryParse),
			["sbyte"] = CreateParseValueDelegate<sbyte>(sbyte.TryParse),
			["short"] = CreateParseValueDelegate<short>(short.TryParse),
			["long"] = CreateParseValueDelegate<long>(long.TryParse),
			["uint"] = CreateParseValueDelegate<uint>(uint.TryParse),
			["byte"] = CreateParseValueDelegate<byte>(byte.TryParse),
			["ushort"] = CreateParseValueDelegate<ushort>(ushort.TryParse),
			["ulong"] = CreateParseValueDelegate<ulong>(ulong.TryParse),
			["nint"] = CreateParseValueDelegate<nint>(nint.TryParse),
			["nuint"] = CreateParseValueDelegate<nuint>(nuint.TryParse),
			["float"] = CreateParseValueDelegate<float>(float.TryParse),
			["double"] = CreateParseValueDelegate<double>(double.TryParse),
			["decimal"] = CreateParseValueDelegate<decimal>(decimal.TryParse),
			["char"] = CreateParseValueDelegate<char>(char.TryParse),
			["bool"] = CreateParseValueDelegate<bool>(bool.TryParse)
		};

		public static bool IsInteger(object? arg)
			=> arg is sbyte or short or int or long or byte or ushort or uint or ulong;

		public static bool IsFloatingPoint(object? arg)
			=> arg is float or double or decimal;

		public static bool ShouldUpcast(INumber target, INumber number) {
			// Integers should always be upcast to floats
			if (target is IInteger && number is IFloat)
				return true;

			// Signed integers should always be upcast to unsigned integers of the same size
			if (target is IInteger && number is IUnsignedInteger && target.NumericSize == number.NumericSize)
				return true;

			// Upcast if the target's size is smaller than the number's size, but prevent floats from being "upcast" to integers
			return target.NumericSize < number.NumericSize && !(target is IFloat && number is IInteger);
		}
	}
}
