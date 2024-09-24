using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using Chips.Common.Utility;
using Chips.Common.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Chips.Compiler {
	internal static partial class InstructionHelper {
		private static readonly ConditionalWeakTable<CilMethodBody, Dictionary<string, int>> _namedBodyLocals = new();

		public static void EmitNumber(this CompilationContext context, sbyte value) => EmitSmallInteger(context, value);

		public static void EmitNumber(this CompilationContext context, byte value) => EmitSmallInteger(context, value);

		public static void EmitNumber(this CompilationContext context, short value) => EmitSmallInteger(context, value);

		public static void EmitNumber(this CompilationContext context, ushort value) => EmitSmallInteger(context, value);

		public static void EmitNumber(this CompilationContext context, int value) => EmitSmallInteger(context, value);

		public static void EmitNumber(this CompilationContext context, uint value) => EmitLargeInteger(context, value);

		public static void EmitNumber(this CompilationContext context, long value) => EmitLargeInteger(context, value);

		public static void EmitNumber(this CompilationContext context, ulong value) => EmitLargeInteger(context, value);

		public static void EmitNumber(this CompilationContext context, nint value) => EmitLargeInteger(context, value);

		public static void EmitNumber(this CompilationContext context, nuint value) => EmitLargeInteger(context, value);

		public static void EmitNumber(this CompilationContext context, float value) => context.Cursor.Emit(CilOpCodes.Ldc_R4, value);

		public static void EmitNumber(this CompilationContext context, double value) => context.Cursor.Emit(CilOpCodes.Ldc_R8, value);

		public static void EmitNumber(this CompilationContext context, decimal value) => context.Cursor.Instructions.LoadDecimalConstant(value, context.importer);

		private static void EmitSmallInteger(CompilationContext context, int value) {
			switch (value) {
				case -1:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_M1);
					break;
				case 0:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_0);
					break;
				case 1:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_1);
					break;
				case 2:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_2);
					break;
				case 3:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_3);
					break;
				case 4:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_4);
					break;
				case 5:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_5);
					break;
				case 6:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_6);
					break;
				case 7:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_7);
					break;
				case 8:
					context.Cursor.Emit(CilOpCodes.Ldc_I4_8);
					break;
				default:
					if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
						context.Cursor.Emit(CilOpCodes.Ldc_I4_S, (sbyte)value);
					else
						context.Cursor.Emit(CilOpCodes.Ldc_I4, value);
					break;
			}
		}

		private static void EmitLargeInteger(CompilationContext context, uint value) {
			if (value <= int.MaxValue)
				EmitSmallInteger(context, (int)value);
			else
				context.Cursor.Emit(CilOpCodes.Ldc_I4, unchecked((int)value));
		}

		private static void EmitLargeInteger(CompilationContext context, long value) {
			if (value >= int.MinValue && value <= int.MaxValue) {
				EmitSmallInteger(context, (int)value);
				context.Cursor.Emit(CilOpCodes.Conv_I8);
			} else
				context.Cursor.Emit(CilOpCodes.Ldc_I8, value);
		}

		private static void EmitLargeInteger(CompilationContext context, ulong value) {
			if (value <= int.MaxValue) {
				EmitSmallInteger(context, (int)value);
				context.Cursor.Emit(CilOpCodes.Conv_U8);
			} else
				context.Cursor.Emit(CilOpCodes.Ldc_I8, unchecked((long)value));
		}

		public static void EmitLargeInteger(CompilationContext context, nint value) {
			if (value >= int.MinValue && value <= int.MaxValue) {
				EmitSmallInteger(context, (int)value);
				context.Cursor.Emit(CilOpCodes.Conv_I);
			} else
				context.Cursor.Emit(CilOpCodes.Ldc_I8, value);
		}

		public static void EmitLargeInteger(CompilationContext context, nuint value) {
			if (value <= int.MaxValue) {
				EmitSmallInteger(context, (int)value);
				context.Cursor.Emit(CilOpCodes.Conv_U);
			} else
				context.Cursor.Emit(CilOpCodes.Ldc_I8, unchecked((long)value));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name) {
			context.Cursor.Emit(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name)
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetFullGenericTypeName()}\" does not have a {name} method")));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name, Type[] arguments) {
			context.Cursor.Emit(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name, arguments)
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetFullGenericTypeName()}\" does not have a {name} method with the provided argument types")));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name, int genericArgumentCount, Type[] arguments) {
			context.Cursor.Emit(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name, genericArgumentCount, arguments)
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetFullGenericTypeName()}\" does not have a {name} method with the provided argument types")));
		}

		private static readonly SignatureComparer _signatureComparer = new(SignatureComparisonFlags.AcceptNewerVersions);

		public static int CreateOrGetLocal<T>(this CompilationContext context, string name) => CreateOrGetLocal(context, name, typeof(T));

		public static int CreateOrGetLocal(this CompilationContext context, string name, Type type) {
			var signature = context.importer.ImportTypeSignature(type);
			int index = FindLocalIndex(context, name, signature, out string usedName, out var locals, out var localDict);
			if (index > -1)
				return index;

			var local = new CilLocalVariable(signature);
			locals.Add(local);
			index = local.Index;
			localDict.Add(usedName, index);
			return index;
		}

		private static int FindLocalIndex(CompilationContext context, string name, TypeSignature? signature, out string usedName, out CilLocalVariableCollection locals, out Dictionary<string, int> localDict) {
			ArgumentNullException.ThrowIfNull(name);

			if (!_namedBodyLocals.TryGetValue(context.Cursor.Body, out localDict!) || localDict is null)
				_namedBodyLocals.Add(context.Cursor.Body, localDict = new());

			locals = context.Cursor.Body.LocalVariables;

			string checkName = name;
			int tries = 1;

			while (localDict.TryGetValue(checkName, out int index)) {
				var localSignature = locals[index].VariableType;

				// Signature will be null if the local has a delayed type resolver
				if (signature is not null && localSignature is not null && _signatureComparer.Equals(localSignature, signature)) {
					usedName = checkName;
					return index;
				}

				checkName = name + tries;
				tries++;
			}

			usedName = checkName;
			return -1;
		}
	}
}
