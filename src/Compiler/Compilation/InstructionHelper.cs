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
using Chips.Utility;
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
			var body = context.Cursor.Body;
			var index = context.Cursor.Index;
			context.Cursor.Emit(CilOpCodes.Nop);
			ChipsCompiler.AddDelayedResolver(getResolver(body, index));
		}

		public static void EmitNopAndDelayedResolver<T>(this CompilationContext context) where T : IDelayedInstructionResolver<T> {
			var body = context.Cursor.Body;
			var index = context.Cursor.Index;
			context.Cursor.Emit(CilOpCodes.Nop);
			ChipsCompiler.AddDelayedResolver(T.Create(body, index));
		}

		public static void EmitNopAndDelayedResolver<T, TArg>(this CompilationContext context, TArg arg) where T : IDelayedInstructionResolver<T, TArg> {
			var body = context.Cursor.Body;
			var index = context.Cursor.Index;
			context.Cursor.Emit(CilOpCodes.Nop);
			ChipsCompiler.AddDelayedResolver(T.Create(body, index, arg));
		}

		public static void EmitNopAndDelayedResolver<T, TArg1, TArg2>(this CompilationContext context, TArg1 arg1, TArg2 arg2) where T : IDelayedInstructionResolver<T, TArg1, TArg2> {
			var body = context.Cursor.Body;
			var index = context.Cursor.Index;
			context.Cursor.Emit(CilOpCodes.Nop);
			ChipsCompiler.AddDelayedResolver(T.Create(body, index, arg1, arg2));
		}

		public static void EmitRegisterLoad(this CompilationContext context, string register) {
			context.Cursor.Emit(CilOpCodes.Ldsfld, context.importer.ImportField(typeof(Registers).GetCachedField(register)
				?? throw new InvalidOperationException($"Register \"{register}\" does not exist")));
		}

		public static void EmitNumberRegisterAssignment(this CompilationContext context, Register register, Type arg) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(register.GetType().GetCachedMethod("Set", arg)
				?? throw new InvalidOperationException($"Type \"{register.GetType().GetSimplifiedGenericTypeName()}\" does not have a Set method")));
		}

		public static void EmitNumberRegisterAssignment<TRegister, TArg>(this CompilationContext context) where TRegister : Register {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedMethod("Set", typeof(TArg))
				?? throw new InvalidOperationException($"Type \"{typeof(TRegister).GetSimplifiedGenericTypeName()}\" does not have a Set method with the provided argument type")));
		}

		public static void EmitRegisterValueAssignment(this CompilationContext context, Register register) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(register.GetType().GetCachedProperty("Value")?.SetMethod
				?? throw new InvalidOperationException($"Type \"{register.GetType().GetSimplifiedGenericTypeName()}\" does not have a Value property")));
		}

		public static void EmitRegisterValueAssignment<TRegister>(this CompilationContext context) where TRegister : Register {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedProperty("Value")?.SetMethod
				?? throw new InvalidOperationException($"Type \"{typeof(TRegister).GetSimplifiedGenericTypeName()}\" does not have a Value property")));
		}

		public static void EmitRegisterValueRetrieval(this CompilationContext context, Register register) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(register.GetType().GetCachedProperty("Value")?.GetMethod
				?? throw new InvalidOperationException($"Type \"{register.GetType().GetSimplifiedGenericTypeName()}\" does not have a Value property")));
		}

		public static void EmitRegisterValueRetrieval<TRegister>(this CompilationContext context) where TRegister : Register {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TRegister).GetCachedProperty("Value")?.GetMethod
				?? throw new InvalidOperationException($"Type \"{typeof(TRegister).GetSimplifiedGenericTypeName()}\" does not have a Value property")));
		}

		public static void EmitBoxToUnderlyingType(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CheckedBoxToUnderlyingType))!));
		}

		public static void EmitUnderlyingTypeValueRetrieval<TNumber>(this CompilationContext context) where TNumber : INumber {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(TNumber).GetCachedProperty("ActualValue")?.GetMethod
				?? throw new InvalidOperationException($"Type \"{typeof(TNumber).GetSimplifiedGenericTypeName()}\" does not have an ActualValue property")));
		}

		public static void EmitFlagAssignment(this CompilationContext context, string flag) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)?.SetMethod
				?? throw new InvalidOperationException($"Flag \"{flag}\" does not exist")));
		}

		public static void EmitFlagAssignment(this CompilationContext context, string flag, bool set) {
			context.EmitRegisterLoad(nameof(Registers.F));
			context.Cursor.Emit(set ? CilOpCodes.Ldc_I4_1 : CilOpCodes.Ldc_I4_0);
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)?.SetMethod
				?? throw new InvalidOperationException($"Flag \"{flag}\" does not exist")));
		}

		public static void EmitFlagRetrieval(this CompilationContext context, string flag) {
			context.EmitRegisterLoad(nameof(Registers.F));
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(FlagsRegister).GetCachedProperty(flag)?.GetMethod
				?? throw new InvalidOperationException($"Flag \"{flag}\" does not exist")));
		}

		public static void EmitZero<T>(this CompilationContext context) where T : INumber, INumberConstants<T> {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedProperty(nameof(INumberConstants<T>.Zero))?.GetMethod
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetSimplifiedGenericTypeName()}\" does not have a Zero property")));
		}

		public static void EmitUpcastTo<T>(this CompilationContext context) where T : INumber {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(T).GetCachedMethod(nameof(INumber.Upcast))
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetSimplifiedGenericTypeName()}\" does not have an Upcast method")));
		}

		public static void EmitCastTo<T>(this CompilationContext context) where T : INumber {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod($"CastTo{typeof(T).Name}", 1, ReflectionCache.T.Type)
				?? throw new InvalidOperationException($"Type \"{typeof(ValueConverter).GetSimplifiedGenericTypeName()}\" does not have a CastTo{typeof(T).Name} method")));
		}

		public static void EmitCastToSByte(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToSByte))!));
		}

		public static void EmitCastToByte(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToByte))!));
		}

		public static void EmitCastToInt16(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToInt16))!));
		}

		public static void EmitCastToUInt16(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToUInt16))!));
		}

		public static void EmitCastToInt32(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToInt32))!));
		}

		public static void EmitCastToUInt32(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToUInt32))!));
		}

		public static void EmitCastToInt64(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToInt64))!));
		}

		public static void EmitCastToUInt64(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToUInt64))!));
		}

		public static void EmitCastToIntPtr(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToIntPtr))!));
		}

		public static void EmitCastToUIntPtr(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToUIntPtr))!));
		}

		public static void EmitCastToSingle(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToSingle))!));
		}

		public static void EmitCastToDouble(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToDouble))!));
		}

		public static void EmitCastToDecimal(this CompilationContext context) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(ValueConverter).GetCachedMethod(nameof(ValueConverter.CastToDecimal))!));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name) {
			context.Cursor.Emit(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name)
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetSimplifiedGenericTypeName()}\" does not have a {name} method")));
		}

		public static void EmitFunctionCall<T>(this CompilationContext context, string name, Type[] arguments) {
			context.Cursor.Emit(CilOpCodes.Callvirt, context.importer.ImportMethod(typeof(T).GetCachedMethod(name, arguments)
				?? throw new InvalidOperationException($"Type \"{typeof(T).GetSimplifiedGenericTypeName()}\" does not have a {name} method with the provided argument types")));
		}

		public static void EmitImplementationCall(this CompilationContext context, string name) {
			context.Cursor.Emit(CilOpCodes.Call, context.importer.ImportMethod(typeof(Implementation).GetCachedMethod(name)
				?? throw new InvalidOperationException($"Type \"{typeof(Implementation).GetSimplifiedGenericTypeName()}\" does not have a {name} method")));
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

		public static Exception ThrowNotImplemented(this CompilingOpcode opcode) => new InvalidOperationException($"CompilingOpcode \"{opcode.GetType().Name}\" does not have a Compile implementation");
	}
}
