using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Chips.Runtime.Types {
	public unsafe struct ArithmeticSet {
		[StructLayout(LayoutKind.Explicit, Size = sizeof(double) * 2 + sizeof(Utility.TypeCode))]
		public struct Number : IComparable<Number> {
			#region Fields
			//Signed integers
			[FieldOffset(0)]
			public sbyte value_i8;
			[FieldOffset(0)]
			public short value_i16;
			[FieldOffset(0)]
			public int value_i32;
			[FieldOffset(0)]
			public long value_i64;

			//Unsigned integers
			[FieldOffset(0)]
			public byte value_u8;
			[FieldOffset(0)]
			public ushort value_u16;
			[FieldOffset(0)]
			public uint value_u32;
			[FieldOffset(0)]
			public ulong value_u64;

			//Floating-point numbers
			[FieldOffset(0)]
			public float value_f32;
			[FieldOffset(0)]
			public double value_f64;
			[FieldOffset(0)]
			public decimal value_f128;
			[FieldOffset(0)]
			public Half value_f16;

			//Special numbers
			[FieldOffset(0)]
			public double value_cplx_real;
			[FieldOffset(sizeof(double))]
			public double value_cplx_imaginary;

			[FieldOffset(sizeof(double) * 2)]
			private Utility.TypeCode typeCode;

			//Hash calculating
			[FieldOffset(0)]
			private readonly long hash;
			[FieldOffset(sizeof(double))]
			private readonly long hash2;
			#endregion

			public bool IsInteger => typeCode == Utility.TypeCode.Int8
				|| typeCode == Utility.TypeCode.Int16
				|| typeCode == Utility.TypeCode.Int32
				|| typeCode == Utility.TypeCode.Int64
				|| typeCode == Utility.TypeCode.Uint8
				|| typeCode == Utility.TypeCode.Uint16
				|| typeCode == Utility.TypeCode.Uint32
				|| typeCode == Utility.TypeCode.Uint64;

			public bool IsFloatingPoint => typeCode == Utility.TypeCode.Float
				|| typeCode == Utility.TypeCode.Double
				|| typeCode == Utility.TypeCode.Decimal
				|| typeCode == Utility.TypeCode.Half;

			#region Static Methods
			public static Number Create(object? value)
				=> value switch {
					sbyte s => s,
					short sh => sh,
					int i => i,
					long l => l,
					byte b => b,
					ushort us => us,
					uint ui => ui,
					ulong ul => ul,
					float f => f,
					double d => d,
					decimal dm => dm,
					Half h => h,
					Complex c => c,
					INumber n => Create(n.Value),
					null => throw new ArgumentNullException(nameof(value)),
					_ => throw new ArgumentException($"A value of type <{TypeTracking.GetChipsType(value, throwOnNotFound: false)}> cannot be used as an internal number type", nameof(value))
				};

			public static int CompareNumbers(object? value, object? value2)
				=> Create(value).CompareTo(Create(value2));

			public static bool SetUniverseMatches(Number num, Number num2)
				=> (num.IsInteger && num2.IsInteger) || (num.IsFloatingPoint && num2.IsFloatingPoint) || (num.typeCode == Utility.TypeCode.Complex && num2.typeCode == Utility.TypeCode.Complex);
			#endregion

			#region Implicit Operators
			//Signed integers
			public static implicit operator Number(sbyte value) => new() { value_i8 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(short value) => new() { value_i16 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(int value) => new() { value_i32 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(long value) => new() { value_i64 = value, typeCode = TypeTracking.GetTypeCode(value) };

			//Unsigned integers
			public static implicit operator Number(byte value) => new() { value_u8 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(ushort value) => new() { value_u16 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(uint value) => new() { value_u32 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(ulong value) => new() { value_u64 = value, typeCode = TypeTracking.GetTypeCode(value) };

			//Floating-point numbers
			public static implicit operator Number(float value) => new() { value_f32 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(double value) => new() { value_f64 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(decimal value) => new() { value_f128 = value, typeCode = TypeTracking.GetTypeCode(value) };
			public static implicit operator Number(Half value) => new() { value_f16 = value, typeCode = TypeTracking.GetTypeCode(value) };

			//Special numbers
			public static implicit operator Number(Complex value) => new() { value_cplx_real = value.Real, value_cplx_imaginary = value.Imaginary, typeCode = TypeTracking.GetTypeCode(value) };
			#endregion

			#region Explicit Operators
			//Signed integers
			public static explicit operator sbyte(Number value) {
				value.VerifyTypeCodeMatch<sbyte>();
				return value.value_i8;
			}
			public static explicit operator short(Number value) {
				value.VerifyTypeCodeMatch<short>();
				return value.value_i16;
			}
			public static explicit operator int(Number value) {
				value.VerifyTypeCodeMatch<int>();
				return value.value_i32;
			}
			public static explicit operator long(Number value) {
				value.VerifyTypeCodeMatch<long>();
				return value.value_i64;
			}

			//Unsigned integers
			public static explicit operator byte(Number value) {
				value.VerifyTypeCodeMatch<byte>();
				return value.value_u8;
			}
			public static explicit operator ushort(Number value) {
				value.VerifyTypeCodeMatch<ushort>();
				return value.value_u16;
			}
			public static explicit operator uint(Number value) {
				value.VerifyTypeCodeMatch<uint>();
				return value.value_u32;
			}
			public static explicit operator ulong(Number value) {
				value.VerifyTypeCodeMatch<ulong>();
				return value.value_u64;
			}

			//Floating-point numbers
			public static explicit operator float(Number value) {
				value.VerifyTypeCodeMatch<float>();
				return value.value_f32;
			}
			public static explicit operator double(Number value) {
				value.VerifyTypeCodeMatch<double>();
				return value.value_f32;
			}
			public static explicit operator decimal(Number value) {
				value.VerifyTypeCodeMatch<decimal>();
				return value.value_f128;
			}
			public static explicit operator Half(Number value) {
				value.VerifyTypeCodeMatch<Half>();
				return value.value_f16;
			}

			//Special numbers
			public static explicit operator Complex(Number value) {
				value.VerifyTypeCodeMatch<Complex>();
				return new(value.value_cplx_real, value.value_cplx_imaginary);
			}
			#endregion

			internal object GetUnderlyingValue()
				=> typeCode switch {
					Utility.TypeCode.Int8 => (sbyte)this,
					Utility.TypeCode.Int16 => (short)this,
					Utility.TypeCode.Int32 => (int)this,
					Utility.TypeCode.Int64 => (long)this,
					Utility.TypeCode.Uint8 => (byte)this,
					Utility.TypeCode.Uint16 => (ushort)this,
					Utility.TypeCode.Uint32 => (uint)this,
					Utility.TypeCode.Uint64 => (ulong)this,
					Utility.TypeCode.Float => (float)this,
					Utility.TypeCode.Double => (double)this,
					Utility.TypeCode.Decimal => (decimal)this,
					Utility.TypeCode.Half => (Half)this,
					Utility.TypeCode.Complex => (Complex)this,
					_ => throw new InvalidOperationException("Internal number-type value contained an invalid type code")
				};

			public override string ToString() {
				object obj = GetUnderlyingValue();

				if (obj is Complex c)
					return Formatting.FormatObject(c);

				return obj.ToString()!;
			}

			public override int GetHashCode()
				=> HashCode.Combine(hash, hash2, (int)typeCode);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void VerifyTypeCodeMatch<T>() where T : new() {
				if (typeCode != TypeTracking.GetTypeCode<T>())
					throw new InvalidOperationException($"Cannot convert the arithmetic set (~set) member to type \"{TypeTracking.GetChipsType<T>()}\"");
			}

			public int CompareTo(Number other) {
				object value = GetUnderlyingValue();
				object otherValue = other.GetUnderlyingValue();

				if (value is Complex complex && otherValue is Complex otherComplex) {
					//Organize by reals first, then imaginary
					if (complex.Real != otherComplex.Real)
						return complex.Real.CompareTo(otherComplex.Real);
					else if (complex.Imaginary != otherComplex.Imaginary)
						return complex.Imaginary.CompareTo(otherComplex.Imaginary);
					return 0;
				} else if (value is Complex complex2) {
					if (ValueConverter.AsSignedInteger(otherValue) is long l)
						return complex2.Real.CompareTo(l);
					else if (ValueConverter.AsUnsignedInteger(otherValue) is ulong ul)
						return complex2.Real.CompareTo(ul);
					else if (ValueConverter.AsFloatingPoint(otherValue) is double d)
						return complex2.Real.CompareTo(d);
					else if (otherValue is decimal dm)
						return -dm.CompareTo((decimal)complex2.Real);
					else if (otherValue is Half h)
						return complex2.Real.CompareTo((double)h);
				} else if (otherValue is Complex complex3) {
					if (ValueConverter.AsSignedInteger(value) is long l)
						return complex3.Real.CompareTo(l);
					else if (ValueConverter.AsUnsignedInteger(value) is ulong ul)
						return complex3.Real.CompareTo(ul);
					else if (ValueConverter.AsFloatingPoint(value) is double d)
						return complex3.Real.CompareTo(d);
					else if (value is decimal dm)
						return -dm.CompareTo((decimal)complex3.Real);
					else if (value is Half h)
						return complex3.Real.CompareTo((double)h);
				} else {
					if (ValueConverter.AsSignedInteger(value) is long l) {
						if (ValueConverter.AsSignedInteger(otherValue) is long l2)
							return l.CompareTo(l2);
						else if (ValueConverter.AsUnsignedInteger(otherValue) is ulong ul)
							return ul > long.MaxValue || l < 0 ? -1 : ((ulong)l).CompareTo(ul);
						else if (ValueConverter.AsFloatingPoint(otherValue) is double d)
							return ((double)l).CompareTo(d);
						else if (otherValue is decimal dm)
							return -dm.CompareTo(l);
						else if (otherValue is Half h)
							return ((double)l).CompareTo((double)h);
					} else if (ValueConverter.AsUnsignedInteger(value) is ulong ul) {
						if (ValueConverter.AsSignedInteger(otherValue) is long l2)
							return ul > long.MaxValue || l2 < 0 ? 1 : ul.CompareTo((ulong)l2);
						else if (ValueConverter.AsUnsignedInteger(otherValue) is ulong ul2)
							return ul.CompareTo(ul2);
						else if (ValueConverter.AsFloatingPoint(otherValue) is double d)
							return ((double)ul).CompareTo(d);
						else if (otherValue is decimal dm)
							return -dm.CompareTo(ul);
						else if (otherValue is Half h)
							return ((double)ul).CompareTo((double)h);
					} else if (ValueConverter.AsFloatingPoint(value) is double d) {
						if (ValueConverter.AsSignedInteger(otherValue) is long l2)
							return d.CompareTo(l2);
						else if (ValueConverter.AsUnsignedInteger(otherValue) is ulong ul2)
							return d.CompareTo(ul2);
						else if (ValueConverter.AsFloatingPoint(otherValue) is double d2)
							return d.CompareTo(d2);
						else if (otherValue is decimal dm)
							return -dm.CompareTo(d);
						else if (otherValue is Half h)
							return d.CompareTo((double)h);
					} else if (value is decimal dm) {
						if (ValueConverter.AsSignedInteger(otherValue) is long l2)
							return dm.CompareTo(l2);
						else if (ValueConverter.AsUnsignedInteger(otherValue) is ulong ul2)
							return dm.CompareTo(ul2);
						else if (ValueConverter.AsFloatingPoint(otherValue) is double d2)
							return dm.CompareTo((decimal)d2);
						else if (otherValue is decimal dm2)
							return dm.CompareTo(dm2);
						else if (otherValue is Half h)
							return dm.CompareTo((decimal)(double)h);
					} else if (value is Half h) {
						if (ValueConverter.AsSignedInteger(otherValue) is long l2)
							return ((double)h).CompareTo(l2);
						else if (ValueConverter.AsUnsignedInteger(otherValue) is ulong ul2)
							return ((double)h).CompareTo(ul2);
						else if (ValueConverter.AsFloatingPoint(otherValue) is double d2)
							return ((double)h).CompareTo(d2);
						else if (otherValue is decimal dm2)
							return -dm2.CompareTo((decimal)(double)h);
						else if (otherValue is Half h2)
							return h.CompareTo(h2);
					}
				}

				throw new InvalidOperationException("Unable to compare two Number instances");
			}
		}

		private readonly Number[] set;

		public static readonly ArithmeticSet EmptySet = new();
		private static bool emptySetInitialized = false;

		public int NumberCount => set.Length;

		public ArithmeticSet() : this(Array.Empty<Number>()) { }

		public ArithmeticSet(params Number[] numbers) {
			if (numbers is null)
				throw new ArgumentNullException(nameof(numbers));

			if (numbers.Length == 0) {
				if (emptySetInitialized)
					set = EmptySet.set;
				else {
					set = Array.Empty<Number>();
					emptySetInitialized = true;
				}
			} else {
				set = (Number[])numbers.Clone();

				OrganizeSet();
			}
		}

		public ArithmeticSet(params object[] numbers) : this(ConvertObjectsToNumbers(numbers)) { }

		public ArithmeticSet(Array array) : this(ConvertObjectsToNumbers(array)) { }

		private static unsafe Number[] ConvertObjectsToNumbers(object[] numbers) {
			Number[] arr = new Number[numbers.Length];

			fixed (Number* ptr = arr) {
				Number* nfPtr = ptr;

				object obj;
				for (int i = 0; i < arr.Length; i++, nfPtr++)
					*nfPtr = (obj = numbers[i]) is Number number ? number : Number.Create(obj);
			}

			return arr;
		}

		private static unsafe Number[] ConvertObjectsToNumbers(Array numbers) {
			Number[] arr = new Number[numbers.Length];

			fixed (Number* ptr = arr) {
				Number* nfPtr = ptr;

				object? obj;
				for (int i = 0; i < arr.Length; i++, nfPtr++)
					*nfPtr = (obj = numbers.GetValue(i)) is Number number ? number : Number.Create(obj);
			}

			return arr;
		}

		public bool IsEmptySet => set.Length == 0;

		private static Number[] ConcatSets(ArithmeticSet a, ArithmeticSet b) {
			var concat = new Number[a.set!.Length + b.set!.Length];
			Array.Copy(a.set, 0, concat, 0, a.set.Length);
			Array.Copy(b.set, 0, concat, a.set.Length, b.set.Length);
			return concat;
		}

		public static ArithmeticSet Union(ArithmeticSet a, ArithmeticSet b) {
			if (b.IsEmptySet)
				return new(a.set);

			ArithmeticSet ret = new(ConcatSets(a, b));
			ret.OrganizeSet();
			return ret;
		}

		public static ArithmeticSet Intersection(ArithmeticSet a, ArithmeticSet b) {
			if (b.IsEmptySet)
				return new();

			ArithmeticSet ret;

			fixed (Number* ptr = b.set) {
				Number* nfPtr = ptr;

				HashSet<Number> hashA = new(a.set);
				HashSet<Number> hash = new();
				for (int i = 0; i < b.set.Length; i++, nfPtr++) {
					if (hashA.Contains(*nfPtr))
						hash.Add(*nfPtr);
				}

				ret = new(hash.ToArray());
			}

			return ret;
		}

		public static ArithmeticSet Difference(ArithmeticSet a, ArithmeticSet b) {
			if (b.IsEmptySet)
				return new(a.set);

			ArithmeticSet ret;

			fixed (Number* ptr = b.set) {
				Number* nfPtr = ptr;

				HashSet<Number> hash = new(a.set);
				for (int i = 0; i < b.set.Length; i++, nfPtr++) {
					if (hash.Contains(*nfPtr))
						hash.Remove(*nfPtr);
				}

				ret = new(hash.ToArray());
			}

			return ret;
		}

		public static ArithmeticSet Complement(ArithmeticSet a, ArithmeticSet universe)
			=> Difference(universe, a);

		public static bool AreDisjoint(ArithmeticSet a, ArithmeticSet b)
			=> Intersection(a, b).IsEmptySet;

		public unsafe object[] ToArray() {
			object[] arr = new object[set.Length];

			fixed (Number* ptr = set) {
				Number* nfPtr = ptr;

				for (int i = 0; i < set.Length; i++, nfPtr++)
					arr[i] = nfPtr->GetUnderlyingValue();
			}

			return arr;
		}

		private void OrganizeSet() {
			Sorting.TimSort(set!);
		}

		public bool ContainsNumber(object? obj) {
			//Performs error checking if "obj" isn't a number
			Number num = Number.Create(obj);

			if (IsEmptySet)
				return false;

			for (int i = 0; i < set.Length; i++)
				if (set[i].CompareTo(num) == 0)
					return true;

			return false;
		}

		public override string ToString() {
			if (set.Length == 0)
				return "{ Empty set }";

			StringBuilder sb = new(Formatting.FormatArray(set).Replace('[', '{').Replace(']', '}'));

			return sb.ToString();
		}

		public override bool Equals(object? obj)
			=> obj is ArithmeticSet set && Difference(this, set).IsEmptySet;

		public override int GetHashCode()
			=> base.GetHashCode();

		public static unsafe bool operator ==(ArithmeticSet set, ArithmeticSet set2) {
			fixed (Number* ptr = set.set) fixed (Number* ptr2 = set2.set) {
				Number* nfPtr = ptr, nfPtr2 = ptr2;

				int length = Math.Min(set.set.Length, set2.set.Length);

				for (int i = 0; i < length; i++, nfPtr++, nfPtr2++)
					if (!Number.SetUniverseMatches(*nfPtr, *nfPtr2) || nfPtr->CompareTo(*nfPtr2) != 0)
						return false;
			}

			return true;
		}

		public static unsafe bool operator !=(ArithmeticSet set, ArithmeticSet set2) {
			fixed (Number* ptr = set.set) fixed (Number* ptr2 = set2.set) {
				Number* nfPtr = ptr, nfPtr2 = ptr2;

				int length = Math.Min(set.set.Length, set2.set.Length);

				for (int i = 0; i < length; i++, nfPtr++, nfPtr2++)
					if (Number.SetUniverseMatches(*nfPtr, *nfPtr2) && nfPtr->CompareTo(*nfPtr2) == 0)
						return false;
			}

			return true;
		}
	}
}
