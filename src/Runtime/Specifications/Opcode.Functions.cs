using Chips.Runtime.Meta;
using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using System;
using System.Numerics;
using System.Text;
using System.Threading;

#pragma warning disable IDE0060
namespace Chips.Runtime.Specifications {
	public unsafe partial class Opcode {
		internal static class Functions {
			#region Functions - A
			public static void Abs(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Abs().Value;
			}

			public static void Acos(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Acos() as INumber)!.Value;
			}

			public static void Acsh(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Acosh() as INumber)!.Value;
			}

			public static void Add(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Add(arg).Value;
			}

			public static void Aems(FunctionContext context) {
				Metadata.Registers.A.Data = ArithmeticSet.EmptySet;

				Metadata.Flags.Zero = true;
			}

			public static void And(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.And(arg) as INumber)!.Value;
			}

			public static void Art(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a floating-point number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Root(arg) as INumber)!.Value;
			}

			public static void Asin(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Asin() as INumber)!.Value;
			}

			public static void Asl(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.ArithmeticShiftLeft() as INumber)!.Value;
			}

			public static void Asnh(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Asinh() as INumber)!.Value;
			}

			public static void Asr(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.ArithmeticShiftRight() as INumber)!.Value;
			}

			public static void Atan(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Atan() as INumber)!.Value;
			}

			public static void Atnh(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Atanh() as INumber)!.Value;
			}

			public static void Atnt(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a floating-point number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Atan2(arg) as INumber)!.Value;
			}
			#endregion

			#region Functions - B
			public static void Br(FunctionContext context)
				=> throw new InvalidOperationException("Branching opcodes should not be invoked directly");

			public static void Blg(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Log2() as INumber)!.Value;
			}

			public static void Bin(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = a.BinaryRepresentation(false);
			}

			public static void Binz(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = a.BinaryRepresentation(true);
			}

			public static void Bit(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.GetBit(arg) as INumber)!.Value;
			}

			public static void Bits(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.GetBits() as INumber)!.Value;
			}
			#endregion

			#region Functions - C
			public static void Call(FunctionContext context)
				=> throw new InvalidOperationException("Call opcode should not be called directly"
						+ ExceptionHelper.GetContextString(context));

			public static void Caps(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = Console.CapsLock;
			}

			public static void Cast(FunctionContext context) {
				if (context.args[0] is not Type type)
					throw new InvalidOpcodeArgumentException(0, "Value was not a type instance", context);

				// TODO: more conversions

				object? data = Metadata.Registers.A.Data;
				if (type == typeof(object[])) {
					//Allow quick conversion to an <~arr:obj> for easier usage of opcodes like "intp"
					if (data is not object[]) {
						Metadata.ZeroFlagChecks = Metadata.CheckCollections;

						Metadata.Registers.A.Data = ValueConverter.AsObjectArray(data);
					}
				} else if (ValueConverter.BoxToUnderlyingType(data) is not INumber number) {
					Metadata.ZeroFlagChecks = Metadata.CheckCollections | Metadata.CheckStrings;

					//Check for conversions between the various types
					switch (data) {
						case Array array:
							if (type == typeof(string) && array is char[] charArray)
								Metadata.Registers.A.Data = new string(charArray);
							else
								goto castFail;
							break;
						case string str:
							if (type == typeof(char[]))
								Metadata.Registers.A.Data = str.ToCharArray();
							else
								goto castFail;

							Metadata.Flags.Conversion = true;
							break;
						case List list:
							if (type == typeof(object[]))
								Metadata.Registers.A.Data = list.ToArray();
							else
								goto castFail;
							break;
					}
				} else {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;

					if (type == typeof(sbyte))
						Metadata.Registers.A.Data = ValueConverter.CastToSByte_T(number);
					else if (type == typeof(short))
						Metadata.Registers.A.Data = ValueConverter.CastToInt16_T(number);
					else if (type == typeof(int))
						Metadata.Registers.A.Data = ValueConverter.CastToInt32_T(number);
					else if (type == typeof(long))
						Metadata.Registers.A.Data = ValueConverter.CastToInt64_T(number);
					else if (type == typeof(byte))
						Metadata.Registers.A.Data = ValueConverter.CastToByte_T(number);
					else if (type == typeof(ushort))
						Metadata.Registers.A.Data = ValueConverter.CastToUInt16_T(number);
					else if (type == typeof(uint))
						Metadata.Registers.A.Data = ValueConverter.CastToUInt32_T(number);
					else if (type == typeof(ulong))
						Metadata.Registers.A.Data = ValueConverter.CastToUInt64_T(number);
					else if (type == typeof(float))
						Metadata.Registers.A.Data = ValueConverter.CastToSingle_T(number);
					else if (type == typeof(double))
						Metadata.Registers.A.Data = ValueConverter.CastToDouble_T(number);
					else if (type == typeof(decimal))
						Metadata.Registers.A.Data = ValueConverter.CastToDecimal_T(number);
					else if (type == typeof(Half))
						Metadata.Registers.A.Data = ValueConverter.CastToHalf_T(number);
					else if (type == typeof(Complex))
						Metadata.Registers.A.Data = ValueConverter.CastToComplex_T(number);
				}

				return;
castFail:
				throw new InvalidOpcodeArgumentException(0, $"Cannot cast a value of type <{TypeTracking.GetChipsType(data, throwOnNotFound: false)}> to type <{TypeTracking.GetChipsType(type, throwOnNotFound: false)}>", context);
			}

			public static void Cclb(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = (int)Console.BackgroundColor;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

					int color;
					if (ValueConverter.AsSignedInteger(Metadata.Registers.A.Data) is long l)
						color = (int)l;
					else if (ValueConverter.AsUnsignedInteger(Metadata.Registers.A.Data) is ulong u)
						color = (int)u;
					else
						throw new InvalidOperationException($"Internal Chips Error -- {Metadata.Registers.A} contained an integer but also did not contain an integer"
							+ ExceptionHelper.GetContextString(context));

					if (color >= 0 && color <= 15)
						Console.BackgroundColor = (ConsoleColor)color;
					else
						throw new InvalidRegisterValueException("The value in " + Metadata.Registers.A.ToString() + " exceeded the expected values (0 to 15, inclusive)", context);
				}
			}

			public static void Cclf(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = (int)Console.ForegroundColor;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

					int color;
					if (ValueConverter.AsSignedInteger(Metadata.Registers.A.Data) is long l)
						color = (int)l;
					else if (ValueConverter.AsUnsignedInteger(Metadata.Registers.A.Data) is ulong u)
						color = (int)u;
					else
						throw new InvalidOperationException($"Internal Chips Error -- {Metadata.Registers.A} contained an integer but also did not contain an integer"
							+ ExceptionHelper.GetContextString(context));

					if (color >= 0 && color <= 15)
						Console.ForegroundColor = (ConsoleColor)color;
					else
						throw new InvalidRegisterValueException("The value in " + Metadata.Registers.A.ToString() + " exceeded the expected values (0 to 15, inclusive)", context);
				}
			}

			public static void Ceil(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Ceiling().Value;
			}

			public static void Ceq(FunctionContext context) {
				object? obj = Metadata.Registers.A.Data, obj2 = context.args[0];

				if (ValueConverter.BoxToUnderlyingType(obj) is INumber number && ValueConverter.BoxToUnderlyingType(obj2) is INumber number2) {
					if (ArithmeticSet.Number.CompareNumbers(number.Value, number2.Value) == 0)
						Metadata.Flags.Comparison = true;
				} else if (obj is ArithmeticSet set && obj2 is ArithmeticSet set2) {
					if (!ArithmeticSet.Difference(set, set2).IsEmptySet)
						Metadata.Flags.Comparison = true;
				} else if (obj is Types.Range range && obj2 is Types.Range range2) {
					if (range.start == range2.start && range.end == range2.end)
						Metadata.Flags.Comparison = true;
				} else
					throw new InvalidOpcodeArgumentException(0, $"Cannot compare equality between a <{TypeTracking.GetChipsType(obj, throwOnNotFound: false)}> and a <{TypeTracking.GetChipsType(obj2, throwOnNotFound: false)}>", context);
			}

			public static void Cge(FunctionContext context) {
				object? obj = Metadata.Registers.A.Data, obj2 = context.args[0];

				if (ValueConverter.BoxToUnderlyingType(obj) is INumber number && ValueConverter.BoxToUnderlyingType(obj2) is INumber number2) {
					if (ArithmeticSet.Number.CompareNumbers(number.Value, number2.Value) >= 0)
						Metadata.Flags.Comparison = true;
				} else
					throw new InvalidOpcodeArgumentException(0, "Opcode \"cge\" can only be used with number values", context);
			}

			public static void Cgt(FunctionContext context) {
				object? obj = Metadata.Registers.A.Data, obj2 = context.args[0];

				if (ValueConverter.BoxToUnderlyingType(obj) is INumber number && ValueConverter.BoxToUnderlyingType(obj2) is INumber number2) {
					if (ArithmeticSet.Number.CompareNumbers(number.Value, number2.Value) > 0)
						Metadata.Flags.Comparison = true;
				} else
					throw new InvalidOpcodeArgumentException(0, "Opcode \"cgt\" can only be used with number values", context);
			}

			public static void Clc(FunctionContext context) {
				Metadata.Flags.Carry = false;
			}

			public static void Cle(FunctionContext context) {
				object? obj = Metadata.Registers.A.Data, obj2 = context.args[0];

				if (ValueConverter.BoxToUnderlyingType(obj) is INumber number && ValueConverter.BoxToUnderlyingType(obj2) is INumber number2) {
					if (ArithmeticSet.Number.CompareNumbers(number.Value, number2.Value) <= 0)
						Metadata.Flags.Comparison = true;
				} else
					throw new InvalidOpcodeArgumentException(0, "Opcode \"cle\" can only be used with number values", context);
			}

			public static void Cln(FunctionContext context) {
				Metadata.Flags.Conversion = false;
			}

			public static void Clo(FunctionContext context) {
				Metadata.Flags.Comparison = false;
			}

			public static void Clp(FunctionContext context) {
				Metadata.Flags.PropertyAccess = false;
			}

			public static void Clr(FunctionContext context) {
				Metadata.Flags.RegexSuccess = false;
			}

			public static void Cls(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				Console.Clear();
			}

			public static void Clt(FunctionContext context) {
				object? obj = Metadata.Registers.A.Data, obj2 = context.args[0];

				if (ValueConverter.BoxToUnderlyingType(obj) is INumber number && ValueConverter.BoxToUnderlyingType(obj2) is INumber number2) {
					if (ArithmeticSet.Number.CompareNumbers(number.Value, number2.Value) < 0)
						Metadata.Flags.Comparison = true;
				} else
					throw new InvalidOpcodeArgumentException(0, "Opcode \"clt\" can only be used with number values", context);
			}

			public static void Clz(FunctionContext context) {
				Metadata.Flags.Zero = false;
			}

			public static void Cnrb(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				ConsoleColor color = Console.ForegroundColor;
				Console.ResetColor();
				Console.ForegroundColor = color;
			}

			public static void Cnrf(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				ConsoleColor color = Console.BackgroundColor;
				Console.ResetColor();
				Console.BackgroundColor = color;
			}

			public static void Cnwh(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.WindowHeight;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not Int32_T i)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <int> value", context);

					Console.WindowHeight = (int)i.Value;
				}
			}

			public static void Cnww(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.WindowWidth;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not Int32_T i)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <int> value", context);

					Console.WindowWidth = (int)i.Value;
				}
			}

			public static void Conh(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.BufferHeight;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not Int32_T i)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <int> value", context);

					Console.BufferHeight = (int)i.Value;
				}
			}

			public static void Cont(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckStrings;
					Metadata.Registers.S.Data = Console.Title;
				} else {
					if (Metadata.Registers.S.Data is not string s)
						throw new InvalidRegisterTypeException(Metadata.Registers.S.ToString() + " was not a <string> value", context);

					Console.Title = s;
				}
			}

			public static void Conr(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				Console.ResetColor();
			}

			public static void Conw(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.BufferWidth;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not Int32_T i)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <int> value", context);

					Console.BufferWidth = (int)i.Value;
				}
			}

			public static void Cos(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Cos() as INumber)!.Value;
			}

			public static void Cosh(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Cosh() as INumber)!.Value;
			}

			public static void Cpcj(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Complex c)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <~cplx> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = Complex.Conjugate(c);
			}

			public static void Cpco(FunctionContext context) {
				Metadata.Registers.A.Data = Complex.ImaginaryOne;
			}

			public static void Cpi(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Complex c)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <~cplx> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = c.Imaginary;
			}

			public static void Cpnr(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Complex c)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <~cplx> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = new Complex(-c.Real, c.Imaginary);
			}

			public static void Cpo(FunctionContext context) {
				Metadata.Registers.A.Data = new Complex(1, 1);
			}

			public static void Cpr(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Complex c)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <~cplx> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = c.Real;
			}

			public static void Cpro(FunctionContext context) {
				Metadata.Registers.A.Data = Complex.One;
			}

			public static void Cprv(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Complex c)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <~cplx> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = new Complex(c.Imaginary, c.Real);
			}

			public static void Cpz(FunctionContext context) {
				Metadata.Registers.A.Data = Complex.Zero;
				Metadata.Flags.Zero = true;
			}

			public static void Csrv(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.CursorVisible;
				} else {
					if (Metadata.Registers.A.Data is not bool b)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <bool> value", context);

					Console.CursorVisible = b;
				}
			}

			public static void Csrx(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.CursorLeft;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not Int32_T i)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <int> value", context);

					Console.CursorLeft = (int)i.Value;
				}
			}

			public static void Csry(FunctionContext context) {
				if (!OperatingSystem.IsWindows())
					throw new InvalidOperationException("Console opcodes are only supported on Windows"
						+ ExceptionHelper.GetContextString(context));

				if (Metadata.Flags.PropertyAccess) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = Console.CursorTop;
				} else {
					if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not Int32_T i)
						throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <int> value", context);

					Console.CursorTop = (int)i.Value;
				}
			}
			#endregion

			#region Functions - D
			public static void Dex(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.X.Data = (Metadata.Registers.X.Data as INumber)!.Decrement();
			}

			public static void Dey(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.X.Data = (Metadata.Registers.Y.Data as INumber)!.Decrement();
			}

			public static void Div(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Divide(arg).Value;
			}

			#region Extended Opcodes - DateTime
			public static void Dtad(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddDays((double)ValueConverter.CastToDouble_T(arg).Value);
			}

			public static void Dtah(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddHours((double)ValueConverter.CastToDouble_T(arg).Value);
			}

			public static void Dtai(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddMinutes((double)ValueConverter.CastToDouble_T(arg).Value);
			}

			public static void Dtam(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddMilliseconds((double)ValueConverter.CastToDouble_T(arg).Value);
			}

			public static void Dtao(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddMonths((int)ValueConverter.CastToInt32_T(arg).Value);
			}

			public static void Dtat(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddTicks((long)ValueConverter.CastToInt64_T(arg).Value);
			}

			public static void Dtas(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddSeconds((double)ValueConverter.CastToDouble_T(arg).Value);
			}

			public static void Dtay(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.AddYears((int)ValueConverter.CastToInt32_T(arg).Value);
			}

			public static void Dtd(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Day;
			}

			public static void Dte(FunctionContext context) {
				Metadata.Registers.A.Data = DateTime.UnixEpoch;
			}

			public static void Dtfm(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = d.ToString(Metadata.Registers.S.Data as string);
			}

			public static void Dth(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Hour;
			}

			public static void Dti(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Minute;
			}

			public static void Dtm(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Millisecond;
			}

			public static void Dtn(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = DateTime.Now;
			}

			public static void Dto(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Month;
			}

			public static void Dtt(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Ticks;
			}

			public static void Dts(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Second;
			}

			public static void Dty(FunctionContext context) {
				if (Metadata.Registers.A.Data is not DateTime d)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <date> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = d.Year;
			}
			#endregion

			public static void Dup(FunctionContext context) {
				try {
					Metadata.stack.Push(Metadata.stack.Peek());
				} catch (Exception e) {
					throw new Exception(e.Message + ExceptionHelper.GetContextString(context));
				}
			}

			public static void Dupd(FunctionContext context) {
				object? obj = Metadata.stack.Peek();

				try {
					Metadata.stack.Push(obj switch {
						Array a => a.Clone(),
						List l => new List(l.ToArray()),
						ArithmeticSet s => new ArithmeticSet(s.ToArray()),
						_ => obj
					});
				} catch (Exception e) {
					throw new Exception(e.Message + ExceptionHelper.GetContextString(context));
				}
			}
			#endregion

			#region Functions - E
			public static void Ext(FunctionContext context)
				=> throw new InvalidOperationException("Extended opcodes cannot be called directly");

			public static void Err(FunctionContext context) {
				if (context.args[0] is not string msg)
					throw new InvalidOpcodeArgumentException(0, "Value must be a <string> instance", context);

				throw new Exception(msg);
			}

			public static void Exp(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Exp() as INumber)!.Value;
			}
			#endregion

			#region Functions - F
			public static void Flor(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Floor().Value;
			}
			#endregion

			#region Functions - H
			public static void Halt(FunctionContext context) {
				int code;
				if (ValueConverter.AsSignedInteger(Metadata.Registers.X.Data) is long l)
					code = (int)l;
				else if (ValueConverter.AsUnsignedInteger(Metadata.Registers.X.Data) is ulong ul)
					code = (int)ul;
				else
					throw new InvalidOperationException($"Internal Chips Error -- {Metadata.Registers.X} did not contain an integer"
						+ ExceptionHelper.GetContextString(context));

				Environment.Exit(code);
			}
			#endregion

			#region Functions - I
			public static void Idx(FunctionContext context) {
				object arg = context.args[0];

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;

				switch (Metadata.Registers.A.Data) {
					case string s:
						if (arg is char c)
							Metadata.Registers.X.Data = s.IndexOf(c);
						else if (arg is string s2)
							Metadata.Registers.X.Data = s.IndexOf(s2);
						else
							throw new InvalidOpcodeArgumentException(0, $"A value of type <{TypeTracking.GetChipsType(arg, throwOnNotFound: false)}> cannot be used for operation \"idx\" on a <string>", context);
						break;
					case List list:
						Metadata.Registers.X.Data = list.IndexOf(arg);
						break;
					case Array arr:
						Metadata.Registers.X.Data = Array.IndexOf(arr, arg);
						break;
					default:
						throw new InvalidRegisterTypeException("The value in " + Metadata.Registers.A.ToString() + " had an invalid type", context);
				}
			}

			public static void Idxv(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Indexer idx)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an <index> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = idx.value;
			}

			public static void Inc(FunctionContext context) {
				Console.Write(Metadata.Registers.S.Data as string);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = Console.ReadKey().KeyChar.ToString();
			}

			public static void Incb(FunctionContext context) {
				Console.Write(Metadata.Registers.S.Data as string);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = Console.ReadKey(true).KeyChar.ToString();
			}

			public static void Inl(FunctionContext context) {
				Console.Write(Metadata.Registers.S.Data as string);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = Console.ReadLine();
			}

			public static void Intp(FunctionContext context) {
				if (Metadata.Registers.A.Data is not object[] arr)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <object[]> value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = string.Format((Metadata.Registers.S.Data as string)!, arr);
			}

			public static void Inv(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Inverse() as INumber)!.Value;
			}

			public static void Inx(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.X.Data = (Metadata.Registers.X.Data as INumber)!.Increment();
			}

			public static void Iny(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.Y.Data = (Metadata.Registers.Y.Data as INumber)!.Increment();
			}

			public static void Is(FunctionContext context) {
				if (context.args[0] is not string type)
					throw new InvalidOpcodeArgumentException(0, "Value must be a type reference", context);

				if (type == "array" && Metadata.Registers.A.Data is Array)
					Metadata.Flags.Comparison = true;
				else if (TypeTracking.GetChipsType(Metadata.Registers.A.Data) == type)
					Metadata.Flags.Comparison = true;
			}

			public static void Isa(FunctionContext context) {
				if (context.args[0] is not string type)
					throw new InvalidOpcodeArgumentException(0, "Value must be a type reference", context);

				if (Metadata.Registers.A.Data is Array array && TypeTracking.GetChipsType(array.GetType().GetElementType()!) == type)
					Metadata.Flags.Comparison = true;
			}
			#endregion

			#region Functions - L
			public static void Lda(FunctionContext context)
				=> throw new InvalidOperationException("Lda opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Ldrg(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckCollections;
				Metadata.Registers.A.Data = Metadata.programArgs;
			}

			public static void Len(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = Metadata.Registers.A.Data switch {
					Array arr => arr.Length,
					ArithmeticSet set => set.NumberCount,
					List _ => throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " contained a <list> instance.  To get the count of a <list>, use the \"lscp\" instruction instead", context),
					null => throw new ArgumentNullException(Metadata.Registers.A.ToString(), "Value was null"
						+ ExceptionHelper.GetContextString(context)),
					_ => throw new InvalidRegisterTypeException($"Cannot perform operation \"length\" on a <{TypeTracking.GetChipsType(Metadata.Registers.A.Data, throwOnNotFound: false)}> value", context)
				};
			}

			public static void Lens(FunctionContext context) {
				if (Metadata.Registers.A.Data is not List list)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <list> instance", context);

				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.Y.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.Y.ToString() + " was not an integer", context);

				var aNum = (a as INumber)!;
				if (ArithmeticSet.Number.CompareNumbers(aNum.Value, 0) < 0 || ArithmeticSet.Number.CompareNumbers(aNum.Value, int.MaxValue) > 0)
					throw new InvalidOperationException("Value was not valid for List.Capacity"
						+ ExceptionHelper.GetContextString(context));

				int newSize = (int)ValueConverter.CastToInt32_T(aNum).Value;
				list.Capacity = newSize;
			}

			public static void Ln(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Ln() as INumber)!.Value;
			}

			public static void Log(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Log10() as INumber)!.Value;
			}

			public static void Lscp(FunctionContext context) {
				if (Metadata.Registers.A.Data is not List list)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <list> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = list.Capacity;
			}

			public static void Lsct(FunctionContext context) {
				if (Metadata.Registers.A.Data is not List list)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <list> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = list.Count;
			}
			#endregion

			#region Functions - M
			public static void Mov(FunctionContext context) {
				if (context.args.Length != 4)
					throw new InvalidOperationException("Invalid opcode parameter list detected (count != 4)"
						+ ExceptionHelper.GetContextString(context));

				if (context.args[0] is not SetValueIndirectly setFunc)
					throw new InvalidOpcodeArgumentException(0, $"Value did not refer to a \"{nameof(SetValueIndirectly)}\" delegate", context);
				if (context.args[2] is not GetValueIndirectly getFunc)
					throw new InvalidOpcodeArgumentException(2, $"Value did not refer to a \"{nameof(GetValueIndirectly)}\" delegate", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats | Metadata.CheckCollections | Metadata.CheckStrings;
				setFunc(ref context.args[1]!, getFunc(context.args[3]));
			}

			public static void Mul(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Multiply(arg).Value;
			}
			#endregion

			#region Functions - N
			public static void Neg(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Negate().Value;
			}

			public static void New(FunctionContext context) {
				//arg0: type
				//arg1: arr -- if opcode uses it
				if (context.args[0] is not string type)
					throw new InvalidOpcodeArgumentException(0, "Value was not a type reference", context);

				if (context.args.Length > 1) {
					switch (type) {
						case "date":
							switch (context.args[1]) {
								case long ticks:
									Metadata.stack.Push(new DateTime(ticks));
									break;
								case int[] values:
									if (values.Length == 3)
										Metadata.stack.Push(new DateTime(values[0], values[1], values[2]));
									else if (values.Length == 6)
										Metadata.stack.Push(new DateTime(values[0], values[1], values[2], values[3], values[4], values[5]));
									else if (values.Length == 7)
										Metadata.stack.Push(new DateTime(values[0], values[1], values[2], values[3], values[4], values[5], values[6]));
									else
										throw new InvalidOpcodeArgumentException(1, "Value (<int[]> instance) had an invalid amount of members for <date> value creation", context);
									break;
								default:
									throw new InvalidOpcodeArgumentException(1, "Value was not a <long> or <int[]>", context);
							}
							break;
						case "range":
							if (ValueConverter.BoxToUnderlyingType(context.args[1]) is not IInteger seed)
								throw new InvalidOpcodeArgumentException(1, "Value was not an integer", context);

							Metadata.stack.Push(new Random((int)ValueConverter.CastToInt32_T((seed as INumber)!).Value));
							break;
						case "set":
							if (context.args[1] is not Array array)
								throw new InvalidOpcodeArgumentException(1, "Value was not an array", context);

							Metadata.stack.Push(new ArithmeticSet(array));
							break;
						case "time":
							switch (context.args[1]) {
								case long ticks:
									Metadata.stack.Push(new TimeSpan(ticks));
									break;
								case int[] values:
									if (values.Length == 3)
										Metadata.stack.Push(new TimeSpan(values[0], values[1], values[2]));
									else if (values.Length == 4)
										Metadata.stack.Push(new TimeSpan(values[0], values[1], values[2], values[3]));
									else if (values.Length == 5)
										Metadata.stack.Push(new TimeSpan(values[0], values[1], values[2], values[3], values[4]));
									else
										throw new InvalidOpcodeArgumentException(1, "Value (<int[]> instance) had an invalid amount of members for <time> value creation", context);
									break;
								default:
									throw new InvalidOpcodeArgumentException(1, "Value was not an <i64> or <int[]>", context);
							}
							break;
						default:
							throw new InvalidOpcodeArgumentException(0, "Could not determine type/function from argument", context);
					}
				} else {
					//Type only
					switch (type) {
						case "index":
							if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.X.Data) is not IInteger value)
								throw new InvalidRegisterTypeException(Metadata.Registers.X.ToString() + " was not an integer", context);

							Metadata.stack.Push(new Indexer((int)ValueConverter.CastToInt32_T((value as INumber)!).Value));
							break;
						case "date":
							Metadata.stack.Push(DateTime.MinValue);
							break;
						case "list":
							if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.Y.Data) is not IInteger capacity)
								throw new InvalidRegisterTypeException(Metadata.Registers.Y.ToString() + " was not an integer", context);

							Metadata.stack.Push(new List((int)ValueConverter.CastToInt32_T((capacity as INumber)!).Value));
							break;
						case "rand":
							Metadata.stack.Push(new Random());
							break;
						case "range":
							if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.X.Data) is not IInteger start)
								throw new InvalidRegisterTypeException(Metadata.Registers.X.ToString() + " was not an integer", context);
							if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.Y.Data) is not IInteger end)
								throw new InvalidRegisterTypeException(Metadata.Registers.Y.ToString() + " was not an integer", context);

							Metadata.stack.Push(new Types.Range((int)ValueConverter.CastToInt32_T((start as INumber)!).Value, (int)ValueConverter.CastToInt32_T((end as INumber)!).Value));
							break;
						case "regex":
							if (Metadata.Registers.S.Data is not string pattern)
								throw new InvalidRegisterTypeException(Metadata.Registers.S.ToString() + " was not a <string> instance", context);

							Metadata.stack.Push(new Types.Regex(pattern));
							break;
						case "set":
							Metadata.stack.Push(ArithmeticSet.EmptySet);
							break;
						case "time":
							Metadata.stack.Push(TimeSpan.Zero);
							break;
						case string a when a.EndsWith("[]"):
							if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.X.Data) is not IInteger length)
								throw new InvalidRegisterTypeException(Metadata.Registers.X.ToString() + " was not an integer", context);

							string subType = a[..^2];
							Metadata.stack.Push(Array.CreateInstance(TypeTracking.GetCSharpType(subType) ?? throw new InvalidOpcodeArgumentException(0, "Type cannot be used as an array element type: " + subType, context), (int)ValueConverter.CastToInt32_T((length as INumber)!).Value));
							break;
						default:
							throw new InvalidOpcodeArgumentException(0, "Could not determine type/function from argument", context);
					}
				}
			}

			public static void Not(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.Not() as INumber)!.Value;
			}
			#endregion

			#region Functions - O
			public static void Or(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.Or(arg) as INumber)!.Value;
			}
			#endregion

			#region Functions - P
			public static void Pntl(FunctionContext context) {
				Console.WriteLine(Formatting.FormatObject(Metadata.Registers.A.Data));
			}

			public static void Poa(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats | Metadata.CheckCollections | Metadata.CheckStrings;
				Metadata.Registers.A.Data = Metadata.stack.Pop();

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.CheckRegister(Metadata.Registers.SP);
			}

			public static void Poed(FunctionContext context)
				=> throw new InvalidOperationException("Poed opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Poev(FunctionContext context)
				=> throw new InvalidOperationException("Poev opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Pop(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats | Metadata.CheckCollections | Metadata.CheckStrings;

				if (context.args.Length == 0) {
					Metadata.Registers.SetStackOperationsObject(Metadata.stack.Pop());
				} else if (context.args.Length == 2) {
					if (context.args[0] is SetValueIndirectly func)
						func(ref context.args[1]!, Metadata.stack.Pop());
					else
						throw new InvalidOpcodeArgumentException(0, $"Value did not refer to a \"{nameof(SetValueIndirectly)}\" delegate", context);
				} else
					throw new InvalidOperationException("Invalid opcode parameter list detected (count != 2)"
						+ ExceptionHelper.GetContextString(context));

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.CheckRegister(Metadata.Registers.SP);
			}

			public static void Pos(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = Metadata.stack.Pop();

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.CheckRegister(Metadata.Registers.SP);
			}

			public static void Pow(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Pow(arg) as INumber)!.Value;
			}

			public static void Pox(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.X.Data = Metadata.stack.Pop();

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.CheckRegister(Metadata.Registers.SP);
			}

			public static void Poy(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.Y.Data = Metadata.stack.Pop();

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.CheckRegister(Metadata.Registers.SP);
			}

			public static void Prnt(FunctionContext context) {
				Console.Write(Formatting.FormatObject(Metadata.Registers.A.Data));
			}

			public static void Prse(FunctionContext context) {
				if (context.args[0] is not string type)
					throw new InvalidOpcodeArgumentException(0, "Value was not a type instance", context);

				if (TypeTracking.cachedParseFuncs.TryGetValue(type, out var func)) {
					if (func(Metadata.Registers.S.Data as string, out object? result)) {
						Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats | Metadata.CheckCollections | Metadata.CheckStrings;
						Metadata.Registers.A.Data = result;

						Metadata.Flags.Conversion = true;
					}
				} else
					throw new InvalidOpcodeArgumentException(0, "Type instance either does not have parsing support or does not exist", context);

				// TODO: parsing other types?
			}

			public static void Psa(FunctionContext context) {
				Metadata.stack.Push(Metadata.Registers.A.Data);
			}

			public static void Psev(FunctionContext context)
				=> throw new InvalidOperationException("Psev opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Psh(FunctionContext context) {
				Metadata.stack.Push(context.args[0]);
			}

			public static void Pss(FunctionContext context) {
				Metadata.stack.Push(Metadata.Registers.S.Data);
			}

			public static void Psx(FunctionContext context) {
				Metadata.stack.Push(Metadata.Registers.X.Data);
			}

			public static void Psy(FunctionContext context) {
				Metadata.stack.Push(Metadata.Registers.Y.Data);
			}
			#endregion

			#region Functions - R
			public static void Rem(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Remainder(arg).Value;
			}

			public static void Ret(FunctionContext context)
				=> throw new InvalidOperationException("Ret opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Rge(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Types.Range range)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <range> instance", context);

				if (context.args.Length == 0) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = range.end;
				} else {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger a)
						throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

					Metadata.ZeroFlagChecks = 0;
					Metadata.Registers.A.Data = new Types.Range(range.start, (int)ValueConverter.CastToInt32_T((a as INumber)!).Value);
				}
			}

			public static void Rgs(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Types.Range range)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <range> instance", context);

				if (context.args.Length == 0) {
					Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
					Metadata.Registers.A.Data = range.start;
				} else {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger a)
						throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

					Metadata.ZeroFlagChecks = 0;
					Metadata.Registers.A.Data = new Types.Range((int)ValueConverter.CastToInt32_T((a as INumber)!).Value, range.end);
				}
			}

			public static void Rgxf(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Regex regex)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <regex> instance", context);

				if (context.args[0] is not string replacement)
					throw new InvalidOpcodeArgumentException(0, "Value was not a <string> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = regex.ReplaceString((Metadata.Registers.S.Data as string)!, replacement);
			}

			public static void Rgxm(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Regex regex)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <regex> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = regex.GetMatchString((int)ValueConverter.CastToInt32_T((arg as INumber)!).Value);
			}

			public static void Rgxs(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Regex regex)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <regex> instance", context);

				regex.AttemptToMatch((Metadata.Registers.S.Data as string)!);
			}

			public static void Rndb(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Random rand)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <rand> instance", context);

				if (context.args[0] is not byte[] arr)
					throw new InvalidOpcodeArgumentException(0, "Value was not an <~arr:u8>", context);

				rand.NextBytes(arr);
			}

			public static void Rndd(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Random rand)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <rand> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;

				if (context.args.Length == 0)
					Metadata.Registers.A.Data = rand.NextDouble();
				else if (context.args.Length == 1) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not a floating-point value", context);

					var argNum = arg as INumber;

					double max = (double)ValueConverter.CastToDouble_T(argNum!).Value;

					if (max < 0)
						throw new InvalidOpcodeArgumentException(0, "Max was less than zero", context);

					Metadata.Registers.A.Data = rand.NextDouble() * max;
				} else if (context.args.Length == 2) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not a floating-point value", context);

					var argNum = arg as INumber;

					if (ValueConverter.BoxToUnderlyingType(context.args[1]) is not IFloat arg2)
						throw new InvalidOpcodeArgumentException(1, "Value was not a floating-point value", context);

					var arg2Num = arg2 as INumber;

					double min = (double)ValueConverter.CastToDouble_T(argNum!).Value;
					double max = (double)ValueConverter.CastToDouble_T(arg2Num!).Value;

					if (min > max)
						throw new InvalidOpcodeArgumentException(0, "Min was greater than max", context);

					Metadata.Registers.A.Data = rand.NextDouble() * (max - min) + min;
				} else
					throw new InvalidOperationException("Invalid opcode parameter list detected (count > 2)"
						+ ExceptionHelper.GetContextString(context));
			}

			public static void Rndf(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Random rand)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <rand> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;

				if (context.args.Length == 0)
					Metadata.Registers.A.Data = rand.NextSingle();
				else if (context.args.Length == 1) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not a floating-point value", context);

					var argNum = arg as INumber;

					if (TypeTracking.GetSizeFromNumericType(argNum!.Value.GetType()) > sizeof(float))
						throw new InvalidOpcodeArgumentException(0, "Integer type was larger than an <f32>", context);

					float max = (float)ValueConverter.CastToSingle_T(argNum).Value;

					if (max < 0)
						throw new InvalidOpcodeArgumentException(0, "Max was less than zero", context);

					Metadata.Registers.A.Data = rand.NextSingle() * max;
				} else if (context.args.Length == 2) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IFloat arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not a floating-point value", context);

					var argNum = arg as INumber;

					if (TypeTracking.GetSizeFromNumericType(argNum!.Value.GetType()) > sizeof(float))
						throw new InvalidOpcodeArgumentException(0, "Integer type was larger than an <f32>", context);

					if (ValueConverter.BoxToUnderlyingType(context.args[1]) is not IFloat arg2)
						throw new InvalidOpcodeArgumentException(1, "Value was not a floating-point value", context);

					var arg2Num = arg2 as INumber;

					if (TypeTracking.GetSizeFromNumericType(arg2Num!.Value.GetType()) > sizeof(float))
						throw new InvalidOpcodeArgumentException(1, "Integer type was larger than an <f32>", context);

					float min = (float)ValueConverter.CastToSingle_T(argNum).Value;
					float max = (float)ValueConverter.CastToSingle_T(arg2Num).Value;

					if (min > max)
						throw new InvalidOpcodeArgumentException(0, "Min was greater than max", context);

					Metadata.Registers.A.Data = rand.NextSingle() * (max - min) + min;
				} else
					throw new InvalidOperationException("Invalid opcode parameter list detected (count > 2)"
						+ ExceptionHelper.GetContextString(context));
			}

			public static void Rndi(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Random rand)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <rand> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;

				if (context.args.Length == 0)
					Metadata.Registers.A.Data = rand.Next();
				else if (context.args.Length == 1) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

					var argNum = arg as INumber;

					if (TypeTracking.GetSizeFromNumericType(argNum!.Value.GetType()) > sizeof(int))
						throw new InvalidOpcodeArgumentException(0, "Integer type was larger than an <int>", context);

					Metadata.Registers.A.Data = rand.Next((int)ValueConverter.CastToInt32_T(argNum).Value);
				} else if (context.args.Length == 2) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

					var argNum = arg as INumber;

					if (TypeTracking.GetSizeFromNumericType(argNum!.Value.GetType()) > sizeof(int))
						throw new InvalidOpcodeArgumentException(0, "Integer type was larger than an <int>", context);

					if (ValueConverter.BoxToUnderlyingType(context.args[1]) is not IInteger arg2)
						throw new InvalidOpcodeArgumentException(1, "Value was not an integer", context);

					var arg2Num = arg2 as INumber;

					if (TypeTracking.GetSizeFromNumericType(arg2Num!.Value.GetType()) > sizeof(int))
						throw new InvalidOpcodeArgumentException(1, "Integer type was larger than an <int>", context);

					Metadata.Registers.A.Data = rand.Next((int)ValueConverter.CastToInt32_T(argNum).Value, (int)ValueConverter.CastToInt32_T(arg2Num).Value);
				} else
					throw new InvalidOperationException("Invalid opcode parameter list detected (count > 2)"
						+ ExceptionHelper.GetContextString(context));
			}

			public static void Rndl(FunctionContext context) {
				if (Metadata.Registers.A.Data is not Random rand)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <rand> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;

				if (context.args.Length == 0)
					Metadata.Registers.A.Data = rand.NextInt64();
				else if (context.args.Length == 1) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

					var argNum = arg as INumber;

					if (argNum!.Value is ulong || argNum.Value is BigInteger)
						throw new InvalidOpcodeArgumentException(0, "Integer type was larger than an <i64>", context);

					Metadata.Registers.A.Data = rand.NextInt64((long)ValueConverter.CastToInt64_T(argNum).Value);
				} else if (context.args.Length == 2) {
					if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
						throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

					var argNum = arg as INumber;

					if (argNum!.Value is ulong || argNum.Value is BigInteger)
						throw new InvalidOpcodeArgumentException(0, "Integer type was larger than an <i64>", context);

					if (ValueConverter.BoxToUnderlyingType(context.args[1]) is not IInteger arg2)
						throw new InvalidOpcodeArgumentException(1, "Value was not an integer", context);

					var arg2Num = arg2 as INumber;

					if (arg2Num!.Value is ulong || arg2Num.Value is BigInteger)
						throw new InvalidOpcodeArgumentException(1, "Integer type was larger than an <i64>", context);

					Metadata.Registers.A.Data = rand.NextInt64((long)ValueConverter.CastToInt64_T(argNum).Value, (long)ValueConverter.CastToInt64_T(arg2Num).Value);
				} else
					throw new InvalidOperationException("Invalid opcode parameter list detected (count > 2)"
						+ ExceptionHelper.GetContextString(context));
			}

			public static void Rol(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.ArithmeticRotateLeft() as INumber)!.Value;
			}

			public static void Ror(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.ArithmeticRotateRight() as INumber)!.Value;
			}
			#endregion

			#region Functions - S
			public static void Sbs(FunctionContext context) {
				int start = (int)ValueConverter.CastToInt32_T(ValueConverter.BoxToUnderlyingType(Metadata.Registers.X.Data)!).Value;
				int count = (int)ValueConverter.CastToInt32_T(ValueConverter.BoxToUnderlyingType(Metadata.Registers.Y.Data)!).Value;

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = (Metadata.Registers.S.Data as string)?.Substring(start, count) ?? throw new InvalidRegisterValueException(Metadata.Registers.S.ToString() + " was null", context);
			}

			public static void Sdiv(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckCollections;

				if (context.args[0] is char c)
					Metadata.Registers.A.Data = (Metadata.Registers.S.Data as string)?.Split(c) ?? throw new InvalidRegisterValueException(Metadata.Registers.S.ToString() + " was null", context);
				else if (context.args[0] is string s)
					Metadata.Registers.A.Data = (Metadata.Registers.S.Data as string)?.Split(s) ?? throw new InvalidRegisterValueException(Metadata.Registers.S.ToString() + " was null", context);
				else
					throw new InvalidOpcodeArgumentException(0, "Value was not a <char> or <string> instance", context);
			}

			public static void Shas(FunctionContext context) {
				if (Metadata.Registers.A.Data is not ArithmeticSet set)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <set> instance", context);

				if (set.ContainsNumber(context.args[0]))
					Metadata.Flags.Comparison = true;
			}

			public static void Sin(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Sin() as INumber)!.Value;
			}

			public static void Sinh(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Sinh() as INumber)!.Value;
			}

			public static void Size(FunctionContext context) {
				int GetSize(object? arg) {
					if (ValueConverter.BoxToUnderlyingType(arg) is INumber a)
						return TypeTracking.GetSizeFromNumericType(a.Value.GetType());
					else if (arg is string s)
						Metadata.Registers.A.Data = s.Length * 2;
					else if (arg is char)
						return sizeof(char);
					else if (arg is Types.Range)
						return sizeof(int) * 2;  //Two ints
					else if (arg is Indexer)
						return sizeof(int);  //One int
					else if (arg is Array arr)
						return arr.Length == 0 ? 8 : GetSize(arr.GetValue(0)) * arr.Length + 8;  //8 == object header size
					else if (arg is TimeSpan)
						return sizeof(long);  //One long
					else if (arg is DateTime)
						return sizeof(ulong);  //One ulong
					else if (arg is bool)
						return sizeof(bool);
					else if (arg is Random)
						return sizeof(int) + sizeof(int) + sizeof(int) * 56 + 8;  //Two ints and one int[56] + object header

					throw new InvalidOperationException($"Cannot get the size of a value of type \"{arg?.GetType().FullName ?? "null"}\""
						+ ExceptionHelper.GetContextString(context));
				}

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = GetSize(Metadata.Registers.A.Data);
			}

			public static void Sjn(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckStrings;

				if (Metadata.Registers.A.Data is char c)
					Metadata.Registers.S.Data = string.Join(c, ValueConverter.AsObjectArray(context.args[0]));
				else if (Metadata.Registers.A.Data is string s)
					Metadata.Registers.S.Data = string.Join(s, ValueConverter.AsObjectArray(context.args[0]));
				else
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <char> or <string> instance", context);
			}

			public static void Sqrt(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Sqrt() as INumber)!.Value;
			}

			public static void Srep(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);

				string? s = Metadata.Registers.S.Data as string;
				if (string.IsNullOrEmpty(s)) {
					Metadata.Flags.Zero = true;
					return;
				}

				int count = (int)ValueConverter.CastToInt32_T((a as INumber)!).Value;

				StringBuilder sb = new(s.Length * count);
				for (int i = 0; i < count; i++)
					sb.Append(s);

				Metadata.ZeroFlagChecks = 0;
				Metadata.Registers.S.Data = sb.ToString();
			}

			public static void Srmv(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckStrings;

				if (Metadata.Registers.A.Data is char c)
					Metadata.Registers.S.Data = (Metadata.Registers.S.Data as string)?.Replace(c.ToString(), "");
				else if (Metadata.Registers.A.Data is string s)
					Metadata.Registers.S.Data = (Metadata.Registers.S.Data as string)?.Replace(s, "");
				else
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <char> or <string> instance", context);
			}

			public static void Stc(FunctionContext context) {
				Metadata.Flags.Carry = true;
			}

			public static void Stco(FunctionContext context) {
				if (Metadata.Registers.A.Data is not ArithmeticSet set)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <set> instance", context);

				if (context.args[0] is not ArithmeticSet universe)
					throw new InvalidOpcodeArgumentException(0, "Value was not a <set> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckCollections;
				Metadata.Registers.A.Data = ArithmeticSet.Complement(set, universe);
			}

			public static void Stdf(FunctionContext context) {
				if (Metadata.Registers.A.Data is not ArithmeticSet set)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <set> instance", context);

				if (context.args[0] is not ArithmeticSet set2)
					throw new InvalidOpcodeArgumentException(0, "Value was not a <set> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckCollections;
				Metadata.Registers.A.Data = ArithmeticSet.Difference(set, set2);
			}

			public static void Stdj(FunctionContext context) {
				if (Metadata.Registers.A.Data is not ArithmeticSet set)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <set> instance", context);

				if (context.args[0] is not ArithmeticSet set2)
					throw new InvalidOpcodeArgumentException(0, "Value was not a <set> instance", context);

				if (ArithmeticSet.AreDisjoint(set, set2))
					Metadata.Flags.Comparison = true;
			}

			public static void Stin(FunctionContext context) {
				if (Metadata.Registers.A.Data is not ArithmeticSet set)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <set> instance", context);

				if (context.args[0] is not ArithmeticSet set2)
					throw new InvalidOpcodeArgumentException(0, "Value was not a <set> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckCollections;
				Metadata.Registers.A.Data = ArithmeticSet.Intersection(set, set2);
			}

			public static void Stp(FunctionContext context) {
				Metadata.Flags.PropertyAccess = true;
			}

			public static void Stun(FunctionContext context) {
				if (Metadata.Registers.A.Data is not ArithmeticSet set)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <set> instance", context);

				if (context.args[0] is not ArithmeticSet set2)
					throw new InvalidOpcodeArgumentException(0, "Value was not a <set> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckCollections;
				Metadata.Registers.A.Data = ArithmeticSet.Union(set, set2);
			}

			public static void Sub(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not INumber a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a number value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers | Metadata.CheckFloats;
				Metadata.Registers.A.Data = a.Subtract(arg).Value;
			}

			public static void Swap(FunctionContext context) {
				object? top = Metadata.stack.Pop();
				object? second = Metadata.stack.Pop();

				Metadata.stack.Push(top);
				Metadata.stack.Push(second);
			}

			public static void Sys(FunctionContext context) {
				if (context.args[0] is not int id)
					throw new InvalidOpcodeArgumentException(0, "Value was not an <int> instance", context);

				switch (id) {
					case 0:
						Metadata.Registers.S.Data = "Chips v" + Sandbox.Version;
						break;
					case 1:
						Console.Write("Press Any Key to Exit... ");
						Console.ReadKey(true);
						Environment.Exit(0);
						break;
					case 2:
						Console.Clear();
						goto case 1;
					default:
						throw new InvalidOpcodeArgumentException(0, "Unknown system call ID: " + id, context);
				}
			}
			#endregion

			#region Functions - T
			public static void Tan(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Tan() as INumber)!.Value;
			}

			public static void Tanh(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IFloat a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a floating-point number value", context);

				Metadata.ZeroFlagChecks = Metadata.CheckFloats;
				Metadata.Registers.A.Data = (a.Tanh() as INumber)!.Value;
			}

			public static void Tas(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = Metadata.Registers.A.Data;
			}

			public static void Tax(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.X.Data = Metadata.Registers.A.Data;
			}

			public static void Tay(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.Y.Data = Metadata.Registers.A.Data;
			}

			#region Extended Opcodes - TimeSpan
			public static void Tmad(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Add(TimeSpan.FromDays((double)ValueConverter.CastToDouble_T(arg).Value));
			}

			public static void Tmah(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Add(TimeSpan.FromHours((double)ValueConverter.CastToDouble_T(arg).Value));
			}

			public static void Tmai(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Add(TimeSpan.FromMinutes((double)ValueConverter.CastToDouble_T(arg).Value));
			}

			public static void Tmam(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Add(TimeSpan.FromMilliseconds((double)ValueConverter.CastToDouble_T(arg).Value));
			}

			public static void Tmas(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Add(TimeSpan.FromSeconds((double)ValueConverter.CastToDouble_T(arg).Value));
			}

			public static void Tmat(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Add(TimeSpan.FromTicks((long)ValueConverter.CastToInt64_T(arg).Value));
			}

			public static void Tmcd(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Days;
			}

			public static void Tmch(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Hours;
			}

			public static void Tmci(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Minutes;
			}

			public static void Tmcm(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Milliseconds;
			}

			public static void Tmcs(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Seconds;
			}

			public static void Tmfm(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.S.Data = time.ToString(Metadata.Registers.S.Data as string);
			}

			public static void Tmt(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.Ticks;
			}

			public static void Tmtd(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.TotalDays;
			}

			public static void Tmth(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.TotalHours;
			}

			public static void Tmti(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.TotalMinutes;
			}

			public static void Tmtm(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.TotalMilliseconds;
			}

			public static void Tmts(FunctionContext context) {
				if (Metadata.Registers.A.Data is not TimeSpan time)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not a <time> instance", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = time.TotalSeconds;
			}
			#endregion

			public static void Tryc(FunctionContext context)
				=> throw new InvalidOperationException("Tryc opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Tryf(FunctionContext context)
				=> throw new InvalidOperationException("Tryf opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Tryn(FunctionContext context)
				=> throw new InvalidOperationException("Tryn opcode should not be called directly"
					+ ExceptionHelper.GetContextString(context));

			public static void Tsa(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckStrings;
				Metadata.Registers.A.Data = Metadata.Registers.S.Data;
			}

			public static void Txa(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = Metadata.Registers.X.Data;
			}

			public static void Txy(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.Y.Data = Metadata.Registers.X.Data;
			}

			public static void Tya(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = Metadata.Registers.Y.Data;
			}

			//Needs a name that isn't "Type" to prevent name clashes
			public static void Type_fn(FunctionContext context) {
				Metadata.ZeroFlagChecks = 0;
				Metadata.Registers.S.Data = TypeTracking.GetChipsType(Metadata.Registers.A.Data);
			}

			public static void Tyx(FunctionContext context) {
				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.X.Data = Metadata.Registers.Y.Data;
			}
			#endregion

			#region Functions - W
			public static void Wait(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not INumber arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not a number", context);

				Thread.Sleep(TimeSpan.FromMilliseconds((double)ValueConverter.CastToDouble_T(arg).Value));
			}
			#endregion

			#region Functions - X
			public static void Xor(FunctionContext context) {
				if (ValueConverter.BoxToUnderlyingType(Metadata.Registers.A.Data) is not IInteger a)
					throw new InvalidRegisterTypeException(Metadata.Registers.A.ToString() + " was not an integer value", context);
				if (ValueConverter.BoxToUnderlyingType(context.args[0]) is not IInteger arg)
					throw new InvalidOpcodeArgumentException(0, "Value was not an integer", context);

				Metadata.ZeroFlagChecks = Metadata.CheckIntegers;
				Metadata.Registers.A.Data = (a.Xor(arg) as INumber)!.Value;
			}
			#endregion
		}
	}
}
