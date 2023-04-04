using Chips.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Chips.Runtime.Utility {
	/// <summary>
	/// A type specializing in faster access of hidden members within types
	/// </summary>
	public static partial class FastReflection {
		public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		// OPTIMIZATION: Store functions by "type.TypeHandle.Value" instead of just "type" due to better GetHashCode() usage
		private static readonly ConcurrentDictionary<nint, ConcurrentDictionary<string, Func<object?, object?>>> getFieldFuncs = new();
		private static readonly ConcurrentDictionary<nint, ConcurrentDictionary<string, Action<object?, object?>>> setFieldFuncs = new();
		private static readonly ConcurrentDictionary<nint, ConcurrentDictionary<string, FieldInfo>> cachedFieldInfos = new();

		private static readonly ConstructorInfo ArgumentException_ctor_string = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string) })!;
		private static readonly ConstructorInfo ArgumentException_ctor_string_string = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(string) })!;

		private static class FieldGenericDelegates<T> {
			public static bool retrieveUnloadingRegistered, assignUnloadingRegistered;
			
			public delegate object? RetrieveDelegate(T instance);
			public delegate void AssignDelegate(ref T instance, object? value);
			
			public static readonly ConcurrentDictionary<string, RetrieveDelegate> getFieldFuncs = new();
			public static readonly ConcurrentDictionary<string, AssignDelegate> setFieldFuncs = new();
		}

		public static object? RetrieveField(this Type type, string fieldName, object? instance) {
			Func<object?, object?> func = BuildRetrieveFieldDelegate(type, fieldName);

			return func(instance);
		}

		public static object? RetrieveField<T>(T instance, string fieldName) {
			FieldGenericDelegates<T>.RetrieveDelegate func = BuildGenericRetrieveFieldDelegate<T>(fieldName);

			return func(instance);
		}

		public static T? RetrieveField<T>(this Type type, string fieldName, object? instance) {
			try {
				return (T)type.RetrieveField(fieldName, instance)!;
			} catch (AmbiguousMatchException) {
				throw;
			} catch (InvalidCastException) {
				throw new InvalidCastException($"Could not cast field \"{type.GetSimplifiedGenericTypeName()}.{fieldName}\" to type \"{typeof(T).GetSimplifiedGenericTypeName()}\"");
			}
		}

		public static F? RetrieveField<T, F>(T instance, string fieldName) {
			try {
				return (F)RetrieveField(instance, fieldName)!;
			} catch (AmbiguousMatchException) {
				throw;
			} catch (InvalidCastException) {
				throw new InvalidCastException($"Could not cast field \"{typeof(T).GetSimplifiedGenericTypeName()}.{fieldName}\" to type \"{typeof(F).GetSimplifiedGenericTypeName()}\"");
			}
		}

		public static object? RetrieveStaticField(this Type type, string fieldName) => RetrieveField(type, fieldName, null);

		public static T? RetrieveStaticField<T>(this Type type, string fieldName) => RetrieveField<T>(type, fieldName, null);

		public static void AssignField(this Type type, string fieldName, object? instance, object? value) {
			Action<object?, object?> func = BuildAssignFieldDelegate(type, fieldName);

			func(instance, value);
		}

		public static void AssignField<T>(ref T instance, string fieldName, object? value) {
			FieldGenericDelegates<T>.AssignDelegate func = BuildGenericAssignFieldDelegate<T>(fieldName);

			func(ref instance, value);
		}

		public static void AssignStaticField(this Type type, string fieldName, object? value) => AssignField(type, fieldName, null, value);

		private static FieldInfo GetField(Type type, string fieldName) {
			nint handle = type.TypeHandle.Value;

			if (cachedFieldInfos.TryGetValue(handle, out var fieldDict) && fieldDict.TryGetValue(fieldName, out FieldInfo? fieldInfo))
				return fieldInfo;

			fieldInfo = type.GetField(fieldName, AllFlags)!;

			if (fieldInfo is null)
				throw new ArgumentException($"Could not find field \"{fieldName}\" in type \"{type.GetSimplifiedGenericTypeName()}\"");

			if (!cachedFieldInfos.TryGetValue(handle, out fieldDict))
				cachedFieldInfos[handle] = fieldDict = new();

			fieldDict[fieldName] = fieldInfo;

			return fieldInfo;
		}

		private static Func<object?, object?> BuildRetrieveFieldDelegate(Type type, string fieldName) {
			ConcurrentDictionary<string, Func<object?, object?>>? funcDictionary;
			Func<object?, object?>? func;

			nint handle = type.TypeHandle.Value;

			if (getFieldFuncs.TryGetValue(handle, out funcDictionary) && funcDictionary.TryGetValue(fieldName, out func))
				return func;

			FieldInfo fieldInfo = GetField(type, fieldName);

			string name = $"{typeof(FastReflection).FullName}.BuildRetrieveFieldDelegate<{type.GetSimplifiedGenericTypeName()}>.get_{fieldName}";
			DynamicMethod method = new(name, typeof(object), new[] { typeof(object) }, type, skipVisibility: true);

			ILGenerator il = method.GetILGenerator();

			Label afterNullCheck = il.DefineLabel();

			if (!fieldInfo.IsStatic) {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Brtrue, afterNullCheck);
				il.Emit(OpCodes.Ldstr, "Cannot load an instance field from a null reference");
				il.Emit(OpCodes.Ldstr, "instance");
				il.Emit(OpCodes.Newobj, ArgumentException_ctor_string_string);
				il.Emit(OpCodes.Throw);

				il.MarkLabel(afterNullCheck);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Unbox_Any, type);
				il.Emit(OpCodes.Ldfld, fieldInfo);
			} else {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Brfalse, afterNullCheck);
				il.Emit(OpCodes.Ldstr, "Cannot load a static field from an object instance");
				il.Emit(OpCodes.Ldstr, "instance");
				il.Emit(OpCodes.Newobj, ArgumentException_ctor_string_string);
				il.Emit(OpCodes.Throw);

				il.MarkLabel(afterNullCheck);

				il.Emit(OpCodes.Ldsfld, fieldInfo);
			}

			if (fieldInfo.FieldType.IsValueType)
				il.Emit(OpCodes.Box, fieldInfo.FieldType);

			il.Emit(OpCodes.Ret);

			if (!getFieldFuncs.TryGetValue(handle, out funcDictionary)) {
				getFieldFuncs[handle] = funcDictionary = new();

				// Capture the parameter into a local
				nint h = handle;

				ALCReflectionUnloader.OnUnload(type.Assembly, () => getFieldFuncs.Remove(h, out _));
			}

			funcDictionary[fieldName] = func = method.CreateDelegate<Func<object?, object>>();

			return func;
		}

		private static Action<object?, object?> BuildAssignFieldDelegate(Type type, string fieldName) {
			ConcurrentDictionary<string, Action<object?, object?>>? funcDictionary;
			Action<object?, object?>? func;

			nint handle = type.TypeHandle.Value;

			if (setFieldFuncs.TryGetValue(handle, out funcDictionary) && funcDictionary.TryGetValue(fieldName, out func))
				return func;

			FieldInfo fieldInfo = GetField(type, fieldName);

			string name = $"{typeof(FastReflection).FullName}.BuildAssignFieldDelegate<{type.GetSimplifiedGenericTypeName()}>.set_{fieldName}";
			DynamicMethod method = new(name, null, new[] { typeof(object), typeof(object) }, type, skipVisibility: true);

			ILGenerator il = method.GetILGenerator();

			Label afterNullCheck = il.DefineLabel();

			if (!fieldInfo.IsStatic) {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Brtrue, afterNullCheck);
				il.Emit(OpCodes.Ldstr, "Cannot assign a value to an instance field from a null reference");
				il.Emit(OpCodes.Ldstr, "instance");
				il.Emit(OpCodes.Newobj, ArgumentException_ctor_string_string);
				il.Emit(OpCodes.Throw);

				il.MarkLabel(afterNullCheck);

				if (type.IsValueType) {
					// Exit prematurely since assigning an object to a copy would be pointless anyway
					il.Emit(OpCodes.Ldstr, $"Cannot modify a field in a copied value type instance.  Use \"{nameof(FastReflection)}.AssignField<T>(ref T, string, object)\" if you want to assign fields in a value type instance.");
					il.Emit(OpCodes.Newobj, ArgumentException_ctor_string);
					il.Emit(OpCodes.Throw);
					goto endOfDelegate;
				}

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Unbox_Any, type);
			} else {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Brfalse, afterNullCheck);
				il.Emit(OpCodes.Ldstr, "Cannot assign a value to a static field from an object instance");
				il.Emit(OpCodes.Ldstr, "instance");
				il.Emit(OpCodes.Newobj, ArgumentException_ctor_string_string);
				il.Emit(OpCodes.Throw);

				il.MarkLabel(afterNullCheck);
			}

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);

			if (!fieldInfo.IsStatic)
				il.Emit(OpCodes.Stfld, fieldInfo);
			else
				il.Emit(OpCodes.Stsfld, fieldInfo);

			endOfDelegate:

			il.Emit(OpCodes.Ret);

			if (!setFieldFuncs.TryGetValue(handle, out funcDictionary)) {
				setFieldFuncs[handle] = funcDictionary = new();

				// Capture the parameter into a local
				nint h = handle;

				ALCReflectionUnloader.OnUnload(type.Assembly, () => setFieldFuncs.Remove(h, out _));
			}

			funcDictionary[fieldName] = func = method.CreateDelegate<Action<object?, object?>>();

			return func;
		}

		private static FieldGenericDelegates<T>.RetrieveDelegate BuildGenericRetrieveFieldDelegate<T>(string fieldName) {
			Type type = typeof(T);
			FieldGenericDelegates<T>.RetrieveDelegate? func;

			if (FieldGenericDelegates<T>.getFieldFuncs.TryGetValue(fieldName, out func))
				return func;

			FieldInfo fieldInfo = GetField(type, fieldName);

			string name = $"{typeof(FastReflection).FullName}.BuildGenericRetrieveFieldDelegate<{type.GetSimplifiedGenericTypeName()}>.get_{fieldName}";
			DynamicMethod method = new(name, typeof(object), new[] { type.MakeByRefType() }, type, skipVisibility: true);

			ILGenerator il = method.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldInfo);

			if (fieldInfo.FieldType.IsValueType)
				il.Emit(OpCodes.Box, fieldInfo.FieldType);

			il.Emit(OpCodes.Ret);

			if (!FieldGenericDelegates<T>.retrieveUnloadingRegistered) {
				FieldGenericDelegates<T>.retrieveUnloadingRegistered = true;

				ALCReflectionUnloader.OnUnload(type.Assembly, FieldGenericDelegates<T>.getFieldFuncs.Clear);
			}

			FieldGenericDelegates<T>.getFieldFuncs[fieldName] = func = method.CreateDelegate<FieldGenericDelegates<T>.RetrieveDelegate>();

			return func;
		}

		private static FieldGenericDelegates<T>.AssignDelegate BuildGenericAssignFieldDelegate<T>(string fieldName) {
			Type type = typeof(T);
			FieldGenericDelegates<T>.AssignDelegate? func;

			if (FieldGenericDelegates<T>.setFieldFuncs.TryGetValue(fieldName, out func))
				return func;

			FieldInfo fieldInfo = GetField(type, fieldName);

			string name = $"{typeof(FastReflection).FullName}.BuildGenericAssignFieldDelegate<{type.GetSimplifiedGenericTypeName()}>.set_{fieldName}";
			DynamicMethod method = new(name, null, new[] { type.MakeByRefType(), typeof(object) }, type, skipVisibility: true);

			ILGenerator il = method.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
			il.Emit(OpCodes.Stfld, fieldInfo);
			il.Emit(OpCodes.Ret);

			if (!FieldGenericDelegates<T>.assignUnloadingRegistered) {
				FieldGenericDelegates<T>.assignUnloadingRegistered = true;

				ALCReflectionUnloader.OnUnload(type.Assembly, FieldGenericDelegates<T>.setFieldFuncs.Clear);
			}

			FieldGenericDelegates<T>.setFieldFuncs[fieldName] = func = method.CreateDelegate<FieldGenericDelegates<T>.AssignDelegate>();

			return func;
		}
	}
}
