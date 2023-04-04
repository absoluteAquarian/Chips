using System;
using System.Numerics;

namespace Chips.Runtime.Utility {
	public enum TypeCode {
		Null = 255,
		/// <summary>
		/// int - signed 32bit integer
		/// </summary>
		Int32 = 0,
		/// <summary>
		/// sbyte - signed 8bit integer
		/// </summary>
		Int8,
		/// <summary>
		/// short - signed 16bit integer
		/// </summary>
		Int16,
		/// <summary>
		/// long - signed 64bit integer
		/// </summary>
		Int64,
		/// <summary>
		/// uint - unsigned 32bit integer
		/// </summary>
		Uint32,
		/// <summary>
		/// byte - unsigned 8bit integer
		/// </summary>
		Uint8,
		/// <summary>
		/// ushort - unsigned 16bit integer
		/// </summary>
		Uint16,
		/// <summary>
		/// ulong - unsigned 64bit integer
		/// </summary>
		Uint64,
		/// <summary>
		/// bigint - <seealso cref="BigInteger"/>
		/// </summary>
		BigInt,
		/// <summary>
		/// float - 32bit floating-point value
		/// </summary>
		Float,
		/// <summary>
		/// double - 64bit floating-point value
		/// </summary>
		Double,
		/// <summary>
		/// decimal - 128bit floating-point value
		/// </summary>
		Decimal,
		/// <summary>
		/// object - object type
		/// </summary>
		Object,
		/// <summary>
		/// char - 2-byte character
		/// </summary>
		Char,
		/// <summary>
		/// string
		/// </summary>
		String,
		/// <summary>
		/// index
		/// </summary>
		Indexer,
		/// <summary>
		/// arrays
		/// </summary>
		Array,
		/// <summary>
		/// range - range of integers
		/// </summary>
		Range,
		/// <summary>
		/// list - dynamic-sized collection of objects
		/// </summary>
		List,
		/// <summary>
		/// time - <seealso cref="TimeSpan"/>
		/// </summary>
		Time,
		/// <summary>
		/// set - arithmetic set of numbers
		/// </summary>
		Set,
		/// <summary>
		/// date - <seealso cref="DateTime"/>
		/// </summary>
		Date,
		/// <summary>
		/// regex - <seealso cref="Types.Regex"/>
		/// </summary>
		Regex,
		/// <summary>
		/// bool - boolean
		/// </summary>
		Bool,
		/// <summary>
		/// range - <seealso cref="System.Random"/>
		/// </summary>
		Random,
		/// <summary>
		/// complex - <seealso cref="System.Numerics.Complex"/>
		/// </summary>
		Complex,
		/// <summary>
		/// Any type that doesn't have a Chips alias
		/// </summary>
		Unknown,
		/// <summary>
		/// half - 16bit floating-point value
		/// </summary>
		Half
	}
}
