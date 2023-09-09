using Chips.Runtime.Meta;
using Chips.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chips.Runtime.Utility {
	public static class TypeTracking {
		private static readonly Dictionary<Type, object> cachedObjects = new();
		private static readonly Dictionary<Type, ChipsGeneratedAttribute?> cachedGeneratedAttribute = new();
		private static readonly Dictionary<Type, Type> cachedArrayTypes = new();
		private static readonly Dictionary<string, Type> cachedNameToTypes = new();

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

		public static bool IsString(object? arg)
			=> arg is string;

		public static bool IsFloatingPoint(object? arg)
			=> arg is float or double or decimal;

		public static Type? GetCSharpType(string chipsType)
			=> chipsType switch {
				"int" => typeof(int),
				"sbyte" => typeof(sbyte),
				"short" => typeof(short),
				"long" => typeof(long),
				"uint" => typeof(uint),
				"byte" => typeof(byte),
				"ushort" => typeof(ushort),
				"ulong" => typeof(ulong),
				"float" => typeof(float),
				"double" => typeof(double),
				"decimal" => typeof(decimal),
				"obj" => typeof(object),
				"char" => typeof(char),
				"string" => typeof(string),
				"bool" => typeof(bool),
				"half" => typeof(Half),
				_ => GetTypeFromAnyAssembly(chipsType) ?? throw new ArgumentException($"Type \"{chipsType}\" does not exist")
			};

		public static Type? GetCSharpType(TypeCode code)
			=> code switch {
				TypeCode.Null => null,
				TypeCode.Int32 => typeof(int),
				TypeCode.Int8 => typeof(sbyte),
				TypeCode.Int16 => typeof(short),
				TypeCode.Int64 => typeof(long),
				TypeCode.Uint32 => typeof(uint),
				TypeCode.Uint8 => typeof(byte),
				TypeCode.Uint16 => typeof(ushort),
				TypeCode.Uint64 => typeof(ulong),
				TypeCode.Float => typeof(float),
				TypeCode.Double => typeof(double),
				TypeCode.Decimal => typeof(decimal),
				TypeCode.Object => typeof(object),
				TypeCode.Char => typeof(char),
				TypeCode.String => typeof(string),
				TypeCode.Bool => typeof(bool),
				_ => throw new ArgumentException("Unknown type code: " + code)
			};

		public static TypeCode GetTypeCode(object? o)
			=> o switch {
				int _ => TypeCode.Int32,
				sbyte _ => TypeCode.Int8,
				short _ => TypeCode.Int16,
				long _ => TypeCode.Int64,
				uint _ => TypeCode.Uint32,
				byte _ => TypeCode.Uint8,
				ushort _ => TypeCode.Uint16,
				ulong _ => TypeCode.Uint64,
				float _ => TypeCode.Float,
				double _ => TypeCode.Double,
				decimal _ => TypeCode.Decimal,
				char _ => TypeCode.Char,
				string _ => TypeCode.String,
				bool _ => TypeCode.Bool,
				null => TypeCode.Null,
				_ => throw new ArgumentException($"Type \"{o.GetType().GetSimplifiedGenericTypeName()}\" does not have an alias")
			};

		public static TypeCode GetTypeCode(Type t) {
			if (!cachedObjects.TryGetValue(t, out object? obj))
				cachedObjects.Add(t, obj = Activator.CreateInstance(t)!);

			return GetTypeCode(obj);
		}

		public static TypeCode GetTypeCode<T>() where T : new() {
			Type t = typeof(T);
			if (!cachedObjects.TryGetValue(t, out object? obj))
				cachedObjects.Add(t, obj = new T());

			return GetTypeCode(obj);
		}

		private static ChipsGeneratedAttribute? GetChipsGeneratedAttribute(this object o) {
			Type t = o.GetType();
			if (!cachedGeneratedAttribute.TryGetValue(t, out ChipsGeneratedAttribute? attr))
				cachedGeneratedAttribute.Add(t, attr = Attribute.GetCustomAttribute(t, typeof(ChipsGeneratedAttribute)) as ChipsGeneratedAttribute);

			return attr;
		}

		internal static int GetSizeFromNumericType(Type t) {
			if (t == typeof(sbyte) || t == typeof(byte))
				return 1;
			if (t == typeof(short) || t == typeof(ushort) || t == typeof(Half))
				return 2;
			if (t == typeof(int) || t == typeof(uint) || t == typeof(float))
				return 4;
			if (t == typeof(long) || t == typeof(ulong) || t == typeof(double))
				return 8;
			if (t == typeof(decimal))
				return 16;

			throw new Exception($"Internal Chips Exception -- Invalid Type for {nameof(TypeTracking)}.{nameof(GetSizeFromNumericType)}: {t.FullName}");
		}

		internal static Type? GetTypeFromAnyAssembly(string name, bool throwOnNotFound = true) {
			Type? search = null;
			string? generatedKey = null;
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
				if (asm.IsDynamic)
					continue;

				search = GetType(name, asm, out generatedKey, throwOnNotFound: false);
				if (search is not null)
					break;
			}

			if (throwOnNotFound && search is null)
				throw new ArgumentException($"Type \"{generatedKey ?? name}\" could not be found");

			return search;
		}

		internal static Type? GetType(string name, Assembly? assembly, out string generatedKey, bool throwOnNotFound = true) {
			string? asm = assembly?.GetName().Name;
			generatedKey = name = (asm is null ? "" : asm + "::") + name;

			if (!cachedNameToTypes.TryGetValue(name, out Type? type)) {
				type = assembly?.GetType(name) ?? Type.GetType(name);

				if (type is not null)
					cachedNameToTypes.Add(name, type!);

				if (throwOnNotFound && type is null)
					throw new ArgumentException($"Type \"{name}\" could not be found");
			}

			return type;
		}
	}
}
