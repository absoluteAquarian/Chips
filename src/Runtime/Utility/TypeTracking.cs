﻿using Chips.Runtime.Types.NumberProcessing;
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

		internal static bool ShouldUpcast(INumber target, INumber number)
			=> (target is IInteger && number is IFloat) || target.NumericSize < number.NumericSize;
	}
}
