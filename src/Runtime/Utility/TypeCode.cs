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
		/// bool - boolean
		/// </summary>
		Bool
	}
}
