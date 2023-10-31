using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler;
using Chips.Compiler.Compilation;
using Chips.Compiler.Utility;
using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using Chips.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Specifications {
	internal static class InstructionHelper {
		private static readonly ConditionalWeakTable<CilMethodBody, Dictionary<string, int>> _namedBodyLocals = new();

		private static readonly Dictionary<OpcodeID, ConstructorInfo> _getConstructorForOpcode = new();

		public static ConstructorInfo GetOrCreateConstructor(this CompilingOpcode opcode, Type[]? parameterTypes = null) {
			var runtimeOpcode = opcode.GetRuntimeOpcode();
			var code = runtimeOpcode.Code;

			if (_getConstructorForOpcode.TryGetValue(code, out var info))
				return info;

			parameterTypes ??= Type.EmptyTypes;

			var constructor = runtimeOpcode.GetType().GetConstructor(BindingFlags.Public | BindingFlags.Instance, parameterTypes)
				?? throw new InvalidOperationException($"Opcode {code} does not have a constructor with the specified parameter types");

			_getConstructorForOpcode.Add(code, constructor);
			return constructor;
		}

		public static void EmitNopAndDelayedResolver(this CompilationContext context, Func<CilMethodBody, int, IDelayedInstructionResolver> getResolver) {
			context.Cursor.Emit(CilOpCodes.Nop);
			var body = context.Cursor.Body;
			var index = context.Cursor.Index;
			ChipsCompiler.AddDelayedResolver(getResolver(body, index));
		}

		public static void EmitRegisterLoad(this CompilationContext context, string register) {
			context.Cursor.Emit(CilOpCodes.Ldsfld, context.importer.ImportField(typeof(Registers).GetCachedField(register)!));
		}

		public static void EmitNumberRegisterAssignment<TRegister, TArg>(this CompilationContext context) where TRegister : Register {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedMethod("Set", typeof(TArg))
				?? throw new InvalidOperationException($"Type {typeof(TRegister).FullName} does not have a Set method")));
		}

		public static void EmitRegisterValueAssignment<TRegister>(this CompilationContext context) where TRegister : Register {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedProperty("Value")!.SetMethod
				?? throw new InvalidOperationException($"Type {typeof(TRegister).FullName} does not have a Value property")));
		}

		public static void EmitRegisterValueRetrieval<TRegister>(this CompilationContext context) where TRegister : Register {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedProperty("Value")!.GetMethod
				?? throw new InvalidOperationException($"Type {typeof(TRegister).FullName} does not have a Value property")));
		}

		public static void EmitINumberValueRetrieval<T>(this CompilationContext context) where T : INumber, INumber<T> {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedProperty(nameof(INumber<T>.ActualValue))!.GetMethod!));
		}

		public static void EmitBoxToUnderlyingType(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CheckedBoxToUnderlyingType))!));
		}

		public static void EmitUnderlyingTypeValueRetrieval<TNumber>(this CompilationContext context) where TNumber : INumber {
			var property = typeof(TNumber).GetCachedProperty("ActualValue");
			if (property?.GetMethod is null)
				throw new InvalidOperationException($"Type {typeof(TNumber).FullName} does not have an ActualValue property");
			else
				context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(property.GetMethod));
		}

		public static void EmitFlagAssignment(this CompilationContext context, string flag) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)!.SetMethod!));
		}

		public static void EmitFlagAssignment(this CompilationContext context, string flag, bool set) {
			context.EmitRegisterLoad(nameof(Registers.F));
			context.Cursor.Emit(set ? CilOpCodes.Ldc_I4_1 : CilOpCodes.Ldc_I4_0);
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)!.SetMethod!));
		}

		public static void EmitFlagRetrieval(this CompilationContext context, string flag) {
			context.EmitRegisterLoad(nameof(Registers.F));
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)!.GetMethod!));
		}

		public static void EmitZero<T>(this CompilationContext context) where T : INumber, INumberConstants<T> {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedProperty("Zero")!.GetMethod!));
		}

		public static void EmitUpcastTo<T>(this CompilationContext context) where T : INumber {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedMethod(nameof(INumber.Upcast))!));
		}

		public static void EmitCastTo<T>(this CompilationContext context) where T : INumber {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod($"CastTo{typeof(T).Name}", 1, ReflectionCache.T.Type)!));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name) {
			context.Cursor.Emit(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name)!));
		}

		private static readonly SignatureComparer _signatureComparer = new(SignatureComparisonFlags.AcceptNewerVersions);

		public static int CreateOrGetLocal<T>(this CompilationContext context, string name) {
			var signature = context.importer.ImportTypeSignature(typeof(T));
			int index = FindLocalIndex(context, name, signature, out string usedName, out var locals, out var localDict);
			if (index > -1)
				return index;

			var local = new CilLocalVariable(signature);
			locals.Add(local);
			index = local.Index;
			localDict.Add(usedName, index);
			return index;
		}

		public static int CreateOrGetLocal(this CompilationContext context, string name, DelayedTypeResolver type) {
			ArgumentNullException.ThrowIfNull(type);
			int index = FindLocalIndex(context, name, null, out string usedName, out var locals, out var localDict);
			if (index > -1)
				return index;

			var local = new CilLocalVariable(null!);
			locals.Add(local);
			index = local.Index;
			localDict.Add(usedName, index);

			var resolver = new DelayedLocalTypeResolver(context.Cursor.Body, index, type);
			ChipsCompiler.AddDelayedResolver(resolver);

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

		public static Exception ThrowNotImplemented(this Opcode opcode) => new InvalidOperationException($"Opcode {opcode.Code} is not implemented, cannot compile");
	}
}
