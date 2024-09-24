using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	internal static partial class ObjectOperations {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TObject Add_Number_Number<TObject, TOrig, TOperand, TResult>(this ref TObject source, TOperand operand)
		where TObject : struct, ICreateObject<TObject>
		where TOrig : struct, INumberBase<TOrig>
		where TOperand : struct, INumberBase<TOperand>
		where TResult : struct, INumberBase<TResult>, IAdditionOperators<TResult, TResult, TResult>
			=> TObject.Create(TResult.CreateChecked(Unsafe.As<TObject, TOrig>(ref source)) + TResult.CreateChecked(operand));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TObject Add_Int_Number_UnknownUpcast<TObject, TOrig, TOperand>(this ref TObject source, TOperand operand)
		where TObject : struct, ICreateObject<TObject>
		where TOrig : struct, INumberBase<TOrig>
		where TOperand : struct, INumberBase<TOperand> {
			if (typeof(TOrig) == typeof(char)) {
				if (typeof(TOperand) == typeof(float) || typeof(TOperand) == typeof(double) || typeof(TOperand) == typeof(decimal) || typeof(TOperand) == typeof(Half))
					throw new InvalidNumberOperandException<TOperand>();

				// Force result to be a char
				return Add_Number_Number<TObject, char, TOperand, char>(ref source, operand);
			} else if (typeof(TOrig) == typeof(ulong))
				// No upcast needed, original type is already ulong
				return Add_Number_Number<TObject, ulong, TOperand, ulong>(ref source, operand);
			else if (typeof(TOrig) == typeof(long)) {
				if (typeof(TOperand) == typeof(ulong))
					return Add_Number_Number<TObject, long, TOperand, ulong>(ref source, operand);  // Upcast to type of argument
				
				return Add_Number_Number<TObject, long, TOperand, long>(ref source, operand);  // Upcast to Int64
			} else if (typeof(TOrig) == typeof(uint)) {
				if (typeof(TOperand) == typeof(long) || typeof(TOperand) == typeof(ulong))
					return Add_Number_Number<TObject, uint, TOperand, TOperand>(ref source, operand);  // Upcast to type of argument
				
				return Add_Number_Number<TObject, uint, TOperand, uint>(ref source, operand);  // Upcast to UInt32
			} else {
				if (typeof(TOperand) == typeof(sbyte) || typeof(TOperand) == typeof(byte) || typeof(TOperand) == typeof(short) || typeof(TOperand) == typeof(ushort) || typeof(TOperand) == typeof(int) || typeof(TOperand) == typeof(char))
					return Add_Number_Number<TObject, TOrig, TOperand, int>(ref source, operand);  // Upcast to Int32
				
				return Add_Number_Number<TObject, TOrig, TOperand, TOperand>(ref source, operand);  // Upcast to type of argument
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TObject Add_Address_Number_UnknownUpcast<TObject, TOrig, TOperand>(this ref TObject source, TOperand operand)
		where TObject : struct, ICreateObject<TObject>
		where TOrig : struct, INumberBase<TOrig>
		where TOperand : struct, INumberBase<TOperand> {
			if (typeof(TOperand) == typeof(char) || typeof(TOperand) == typeof(float) || typeof(TOperand) == typeof(double) || typeof(TOperand) == typeof(decimal) || typeof(TOperand) == typeof(Half))
				throw new InvalidNumberOperandException<TOperand>();

			if (typeof(TOrig) == typeof(nint)) {
				if (typeof(TOperand) == typeof(nuint))
					return Add_Number_Number<TObject, TOrig, TOperand, nuint>(ref source, operand);  // Upcast to type of argument
				
				return Add_Number_Number<TObject, TOrig, TOperand, nint>(ref source, operand);  // Upcast to nint
			}
			
			return Add_Number_Number<TObject, TOrig, TOperand, nuint>(ref source, operand);  // No upcast needed, original type is already nuint
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TObject Add_Float_Number_UnknownUpcast<TObject, TOrig, TOperand>(this ref TObject source, TOperand operand)
		where TObject : struct, ICreateObject<TObject>
		where TOrig : struct, INumberBase<TOrig>
		where TOperand : struct, INumberBase<TOperand> {
			if (typeof(TOperand) == typeof(char))
				throw new InvalidNumberOperandException<TOperand>();

			if (typeof(TOrig) == typeof(Half)) {
				if (typeof(TOperand) == typeof(float) || typeof(TOperand) == typeof(double) || typeof(TOperand) == typeof(decimal))
					return Add_Number_Number<TObject, Half, TOperand, TOperand>(ref source, operand);  // Upcast to type of argument
				
				return Add_Number_Number<TObject, Half, TOperand, float>(ref source, operand);  // Upcast to Half
			} else if (typeof(TOrig) == typeof(float)) {
				if (typeof(TOperand) == typeof(double) || typeof(TOperand) == typeof(decimal))
					return Add_Number_Number<TObject, float, TOperand, TOperand>(ref source, operand);  // Upcast to type of argument
				
				return Add_Number_Number<TObject, float, TOperand, float>(ref source, operand);  // Upcast to Single
			} else if (typeof(TOrig) == typeof(double)) {
				if (typeof(TOperand) == typeof(decimal))
					return Add_Number_Number<TObject, double, TOperand, decimal>(ref source, operand);  // Upcast to type of argument
				
				return Add_Number_Number<TObject, double, TOperand, double>(ref source, operand);  // Upcast to Double
			}
			
			return Add_Number_Number<TObject, decimal, TOperand, decimal>(ref source, operand);  // No upcast needed, original type is already Decimal
		}
	}
}
