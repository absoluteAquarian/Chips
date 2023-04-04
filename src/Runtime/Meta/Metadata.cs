using Chips.Runtime.Specifications;
using Chips.Runtime.Types;
using Chips.Runtime.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Chips.Runtime.Meta {
	public static unsafe class Metadata {
		public static class Registers {
			/// <summary>
			/// The register for arithmetic.  Can contain any type of value
			/// </summary>
			public static readonly Register A = new("A", null, null);
			/// <summary>
			/// The register for storing the current exception
			/// </summary>
			public static readonly Register E = new("E", null, null);
			/// <summary>
			/// The register for indexing.  Can only contain integers
			/// </summary>
			public static readonly Register X = new("X", 0, &TypeTracking.IsInteger);
			/// <summary>
			/// The register for counting.  Can only contain integers
			/// </summary>
			public static readonly Register Y = new("Y", 0, &TypeTracking.IsInteger);
			public static readonly Register SP = new("SP", 0, &StackPointer_ReadOnly) { getDataOverride = &StackPointer_GetValue };
			/// <summary>
			/// The register for string manipulation.  Can only contain strings
			/// </summary>
			public static readonly Register S = new("S", null, &TypeTracking.IsString);

			//Used for checking values popped off of the stack
			private static readonly Register StackOperations = new("<so>", null, null);

			private static bool StackPointer_ReadOnly(object? obj)
				=> throw new InvalidOperationException("Stack pointer cannot be written to");

			private static object? StackPointer_GetValue()
				=> stack.SP;

			/// <summary>
			/// Sets the data of a hidden register for usage with <seealso cref="ZeroFlagChecks"/>
			/// </summary>
			/// <param name="obj">The object</param>
			public static void SetStackOperationsObject(object? obj)
				=> StackOperations.Data = obj;

			/// <summary>
			/// Uses <seealso cref="ZeroFlagChecks"/> to parse <paramref name="register"/> for setting the <seealso cref="Flags.Zero"/> flag
			/// </summary>
			/// <param name="register">The register to check</param>
			public static void CheckRegister(Register register) {
				object? obj = register.Data;

				if (obj is null) {
					Metadata.Flags.Zero = true;
					return;
				}

				var type = obj.GetType();

				bool zeroFlagSuccess_Integer = (ZeroFlagChecks & CheckIntegers) != 0 && type.IsPrimitive
					&& ((obj is char c && c == 0)
					|| (obj is bool b && b)
					|| (obj is DateTime date && date == default)
					|| (obj is TimeSpan time && time == default)
					|| (ValueConverter.AsUnsignedInteger(obj) is ulong ul && ul == 0)
					|| (ValueConverter.AsSignedInteger(obj) is long l && l == 0));
				bool zeroFlagSucess_Float = (ZeroFlagChecks & CheckFloats) != 0
					&& ((obj is Half h && h == (Half)0f)
					|| (obj is Complex cm && cm == Complex.Zero)
					|| (type.IsPrimitive && ValueConverter.AsFloatingPoint(obj) is double d && d == 0d));
				bool zeroFlagSuccess_Collections = (ZeroFlagChecks & CheckCollections) != 0
					&& ((obj is Array array && array.Length == 0)
					|| (obj is List list && list.Count == 0)
					|| (obj is ArithmeticSet set && set.IsEmptySet));
				bool zeroFlagSuccess_String = (ZeroFlagChecks & CheckStrings) != 0 && obj is string str && str == "";

				if (zeroFlagSuccess_Integer || zeroFlagSucess_Float || zeroFlagSuccess_Collections || zeroFlagSuccess_String)
					Flags.Zero = true;

				ZeroFlagChecks = 0b0000;
			}
		}

		public static class Flags {
			private static ushort flags;

			/// <summary>
			/// Flag $C - carry bit
			/// </summary>
			public static bool Carry {
				get => (flags & 0x0001) != 0;
				set => flags = (byte)(value ? flags | 0x0001 : flags & ~0x0001);
			}

			/// <summary>
			/// Flag $N - whether value conversion from a string to some other type was successful
			/// </summary>
			public static bool Conversion {
				get => (flags & 0x0002) != 0;
				set => flags = (byte)(value ? flags | 0x0002 : flags & ~0x0002);
			}

			/// <summary>
			/// Flag $O - value comparison
			/// </summary>
			public static bool Comparison {
				get => (flags & 0x0004) != 0;
				set => flags = (byte)(value ? flags | 0x0004 : flags & ~0x0004);
			}

			/// <summary>
			/// Flag $P - toggles the effect of various instructions from value access to value assignment
			/// </summary>
			public static bool PropertyAccess {
				get => (flags & 0x0008) != 0;
				set => flags = (byte)(value ? flags | 0x0008 : flags & ~0x0008);
			}

			/// <summary>
			/// Flag $R - set when a match is found for a Regex instance
			/// </summary>
			public static bool RegexSuccess {
				get => (flags & 0x0010) != 0;
				set => flags = (byte)(value ? flags | 0x0010 : flags & ~0x0010);
			}

			/// <summary>
			/// Flag $Z - set when certain operations result in a zero value
			/// </summary>
			public static bool Zero {
				get => (flags & 0x0020) != 0;
				set => flags = (byte)(value ? flags | 0x0020 : flags & ~0x0020);
			}
		}

		/// <summary>
		/// Used by the registers when assigning values to their Data properties
		/// <para>
		/// <list type="bullet">
		/// <item>000X - check Integers</item>
		/// <item>00X0 - check Floats</item>
		/// <item>0X00 - check Collections</item>
		/// <item>X000 - check Strings</item>
		/// </list>
		/// </para>
		/// </summary>
		public static int ZeroFlagChecks { get; set; }

		public const byte CheckIntegers = 0b0001;
		public const byte CheckFloats = 0b0010;
		public const byte CheckCollections = 0b0100;
		public const byte CheckStrings = 0b1000;

		public static Stack stack;

		public static string[] programArgs;

		//Used to ensure that no opcode shares the same code
		public static readonly OpcodeTable op;

		public static readonly Dictionary<string, Type> userDefinedTypes = new();

		static Metadata() {
			op = new OpcodeTable();

			foreach (var field in typeof(Opcodes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(Opcode))) {
				Opcode opcode = (field.GetValue(null) as Opcode)!;

				if (opcode.Parent?.table is null && op[opcode.code] is not null)
					throw new Exception($"Opcode 0x{opcode.code:X2} cannot be assigned to the instruction \"{opcode.descriptor}\" since it already assigned to the instruction \"{op[opcode.code].descriptor}\"");

				if (opcode.Parent?.table is not null && opcode.Parent.table[opcode.code] is not null)
					throw new Exception($"Opcode 0x{opcode.Parent.code:X2}{opcode.code:X2} cannot be assigned to the instruction \"{opcode.descriptor}\" since it already assigned to the instruction \"{opcode.Parent.table[opcode.code].descriptor}\"");

				var table = opcode.Parent?.table ?? op;
				table[opcode.code] = opcode;
			}
		}

		public static void SetRegisterValue(ref object? target, object? value) {
			ZeroFlagChecks = CheckIntegers | CheckFloats | CheckCollections | CheckStrings;
			if (target is Register register)
				register.Data = value;
			else
				throw new ArgumentException("Target was not a register", nameof(target));
		}

		public static void SetRegisterValueIndexByX(ref object? target, object? value) {
			if (target is Register register) {
				Registers.X.GetDataAsInt32(out int x);

				var obj = register.Data;
				if (obj is Array array)
					array.SetValue(value, x);
				else if (obj is List list)
					list[x] = value;
				else
					throw new InvalidRegisterTypeException(register.ToString() + " was not an <~arr> or <~list> instance", Opcode.FunctionContext.NoContext);
			} else
				throw new ArgumentException("Target was not a register", nameof(target));
		}

		public static void SetRegisterValueIndexByY(ref object? target, object? value) {
			if (target is Register register) {
				Registers.Y.GetDataAsInt32(out int y);

				var obj = register.Data;
				if (obj is Array array)
					array.SetValue(value, y);
				else if (obj is List list)
					list[y] = value;
				else
					throw new InvalidRegisterTypeException(register.ToString() + " was not an <~arr> or <~list> instance", Opcode.FunctionContext.NoContext);
			} else
				throw new ArgumentException("Target was not a register", nameof(target));
		}

		public static void SetArrayOrListIndexByX(ref object? target, object? value) {
			Registers.X.GetDataAsInt32(out int x);

			if (target is Array array)
				array.SetValue(value, x);
			else if (target is List list)
				list[x] = value;
			else
				throw new ArgumentException("Target was not an <~arr> or <~list> instance");
		}

		public static void SetArrayOrListIndexByY(ref object? target, object? value) {
			Registers.Y.GetDataAsInt32(out int y);

			if (target is Array array)
				array.SetValue(value, y);
			else if (target is List list)
				list[y] = value;
			else
				throw new ArgumentException("Target was not an <~arr> or <~list> instance");
		}

		public static object? GetRegisterValue(object? target) {
			if (target is Register register)
				return register.Data;
			else
				throw new ArgumentException("Target was not a register", nameof(target));
		}

		public static object? GetRegisterValueIndexByX(object? target) {
			if (target is Register register) {
				Registers.X.GetDataAsInt32(out int x);

				var obj = register.Data;
				if (obj is Array array)
					return array.GetValue(x);
				else if (obj is List list)
					return list[x];
				else
					throw new InvalidRegisterTypeException(register.ToString() + " was not an <~arr> or <~list> instance", Opcode.FunctionContext.NoContext);
			} else
				throw new ArgumentException("Target was not a register", nameof(target));
		}

		public static object? GetRegisterValueIndexByY(object? target) {
			if (target is Register register) {
				Registers.Y.GetDataAsInt32(out int y);

				var obj = register.Data;
				if (obj is Array array)
					return array.GetValue(y);
				else if (obj is List list)
					return list[y];
				else
					throw new InvalidRegisterTypeException(register.ToString() + " was not an <~arr> or <~list> instance", Opcode.FunctionContext.NoContext);
			} else
				throw new ArgumentException("Target was not a register", nameof(target));
		}

		public static object? AccessArrayOrListIndexByX(ref object? target) {
			Registers.X.GetDataAsInt32(out int x);

			if (target is Array array)
				return array.GetValue(x);
			else if (target is List list)
				return list[x];
			else
				throw new ArgumentException("Target was not an <~arr> or <~list> instance");
		}

		public static object? AccessArrayOrListIndexByY(ref object? target) {
			Registers.Y.GetDataAsInt32(out int y);

			if (target is Array array)
				return array.GetValue(y);
			else if (target is List list)
				return list[y];
			else
				throw new ArgumentException("Target was not an <~arr> or <~list> instance");
		}
	}
}
