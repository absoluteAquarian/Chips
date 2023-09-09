using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;
using System;
using System.Reflection;

namespace Chips.Utility {
	partial class Extensions {
		private static readonly ConstructorInfo Decimal_ctor_int_int_int_bool_byte = typeof(decimal).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) })!;

		/// <summary>
		/// Pushes a <seealso cref="decimal"/> constant to the evaluation stack
		/// </summary>
		public static void LoadDecimalConstant(this CilInstructionCollection il, decimal number, ReferenceImporter importer) {
			// Extract the integers needed for the constructor
			Span<int> bits = stackalloc int[4];
			decimal.GetBits(number, bits);

			// Extract the sign and scale information
			// N------- ---SSSSS -------- --------
			// N = sign, S = scale
			// All other bits should be zero
			int flags = bits[3];
			bool isNegative = (flags & unchecked((int)0x80000000)) != 0;
			byte scale = (byte)(flags >> 16);

			// Emit the constructor call
			il.Add(CilOpCodes.Ldc_I4, bits[0]);
			il.Add(CilOpCodes.Ldc_I4, bits[1]);
			il.Add(CilOpCodes.Ldc_I4, bits[2]);
			il.Add(isNegative ? CilOpCodes.Ldc_I4_1 : CilOpCodes.Ldc_I4_0);
			il.Add(CilOpCodes.Ldc_I4, scale);
			il.Add(CilOpCodes.Call, importer.ImportMethod(Decimal_ctor_int_int_int_bool_byte));
		}
	}
}
