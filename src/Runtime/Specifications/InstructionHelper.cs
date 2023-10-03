using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler;
using Chips.Compiler.Utility;
using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using Chips.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Specifications {
	internal static class InstructionHelper {
		private static readonly ConditionalWeakTable<CilMethodBody, Dictionary<string, int>> _namedBodyLocals = new();

		public static void EmitDelayedResolver(this CompilationContext context, Func<CilMethodBody, int, IDelayedInstructionResolver> getResolver) {
			context.Instructions.Add(CilOpCodes.Nop);
			var body = context.Body;
			var index = body.Instructions.Count - 1;
			ChipsCompiler.AddDelayedResolver(getResolver(body, index));
		}

		public static void EmitRegisterLoad(this CompilationContext context, string register) {
			context.Instructions.Add(CilOpCodes.Ldsfld, context.importer.ImportField(typeof(Registers).GetCachedField(register)!));
		}

		public static void EmitNumberRegisterAssignment<TRegister, TArg>(this CompilationContext context) where TRegister : Register {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedMethod("Set", typeof(TArg))!));
		}

		public static void EmitRegisterValueAssignment<TRegister>(this CompilationContext context) where TRegister : Register {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedProperty("Value")!.SetMethod!));
		}

		public static void EmitRegisterValueRetrieval<TRegister>(this CompilationContext context) where TRegister : Register {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedProperty("Value")!.GetMethod!));
		}

		public static void EmitBoxToUnderlyingType(this CompilationContext context) {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CheckedBoxToUnderlyingType))!));
		}

		public static void EmitUnderlyingTypeValueRetrieval<TNumber>(this CompilationContext context) where TNumber : INumber {
			var property = typeof(TNumber).GetCachedProperty("ActualValue");
			if (property?.GetMethod is null)
				throw new InvalidOperationException($"Type {typeof(TNumber).FullName} does not have an ActualValue property");
			else
				context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(property.GetMethod));
		}

		public static void EmitFlagAssignment(this CompilationContext context, string flag, bool set) {
			context.EmitRegisterLoad(nameof(Registers.F));
			context.Instructions.Add(set ? CilOpCodes.Ldc_I4_1 : CilOpCodes.Ldc_I4_0);
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)!.SetMethod!));
		}

		public static void EmitFlagRetrieval(this CompilationContext context, string flag) {
			context.EmitRegisterLoad(nameof(Registers.F));
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)!.GetMethod!));
		}

		public static void EmitZero<T>(this CompilationContext context) where T : INumber, INumberConstants<T> {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedProperty("Zero")!.GetMethod!));
		}

		public static void EmitUpcastTo<T>(this CompilationContext context) where T : INumber {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedMethod(nameof(INumber.Upcast))!));
		}

		public static void EmitCastTo<T>(this CompilationContext context) where T : INumber {
			context.Instructions.Add(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod($"CastTo{typeof(T).Name}", 1, ReflectionCache.T.Type)!));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name) {
			context.Instructions.Add(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name)!));
		}

		private static readonly SignatureComparer _signatureComparer = new(SignatureComparisonFlags.AcceptNewerVersions);

		public static int CreateOrGetLocal<T>(this CompilationContext context, string name) {
			ArgumentNullException.ThrowIfNull(name);

			if (!_namedBodyLocals.TryGetValue(context.Body, out var localDict))
				_namedBodyLocals.Add(context.Body, localDict = new());

			var locals = context.Body.LocalVariables;
			var signature = context.importer.ImportTypeSignature(typeof(T));

			string checkName = name;
			int tries = 1;

			while (localDict.TryGetValue(checkName, out int index)) {
				if (_signatureComparer.Equals(locals[index].VariableType, signature))
					return index;

				checkName = name + tries;
				tries++;
			}

			var local = new CilLocalVariable(signature);
			locals.Add(local);
			localDict.Add(checkName, local.Index);
			return local.Index;
		}

		public static Exception ThrowNotImplemented(this Opcode opcode) => new InvalidOperationException($"Opcode {opcode.Code} is not implemented, cannot compile");
	}
}
