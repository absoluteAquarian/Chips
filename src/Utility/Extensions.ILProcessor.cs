using Chips.Utility.Reflection;
using Mono.Cecil.Cil;

namespace Chips.Utility{
	internal static partial class Extensions{
		/// <summary>
		/// Emits IL code which pushes a static method as a <typeparamref name="T"/> delegate to the evaluation stack
		/// </summary>
		/// <typeparam name="T">The type of the delegate</typeparam>
		/// <param name="method">The method reference</param>
		/// <returns>Whether the compilation was successful</returns>
		public static bool PushFunction<T>(this ILProcessor il, string sourceFile, System.Reflection.MethodInfo method) where T : MulticastDelegate{
			if(!method.IsStatic){
				Compiler.exceptions.Add(new(sourceFile, method.DeclaringType!.FullName + "::" + method.Name + " was not a static method"));
				return false;
			}

			var module = il.Body.Method.DeclaringType.Module;

			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Ldftn, module.ImportReference(method));
			//Delegates always have (object, nint) as the arguments for the ctor
			il.Emit(OpCodes.Newobj, module.ImportReference(ReflectionCache.GetCachedConstructor(typeof(T), typeof(object), typeof(nint))));

			return true;
		}

		/// <summary>
		/// Pushes a <seealso cref="decimal"/> constant to the evaluation stack
		/// </summary>
		/// <param name="data">The span passed to <seealso cref="decimal(ReadOnlySpan{int})"/></param>
		/// <returns>Whether the compilation was sucessful</returns>
		public static bool PushDecimalConstant(this ILProcessor il, string sourceFile, ReadOnlySpan<int> data){
			if(data.Length != 4){
				Compiler.exceptions.Add(new(sourceFile, "Span did not have a length of 4"));
				return false;
			}

			var module = il.Body.Method.DeclaringType.Module;

			//IntPtr ptr = stackalloc int[4];
			il.Emit(OpCodes.Ldc_I4_S, 16);
			il.Emit(OpCodes.Conv_U);
			il.Emit(OpCodes.Localloc);

			//*ptr = data[0];
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I4, data[0]);
			il.Emit(OpCodes.Stind_I4);

			//*(ptr + 4) = data[1];
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I4_4);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Ldc_I4, data[1]);
			il.Emit(OpCodes.Stind_I4);

			//*(ptr + (IntPtr)2 * 4) = data[2];
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I4_2);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I4_4);
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Ldc_I4, data[2]);
			il.Emit(OpCodes.Stind_I4);

			//*(ptr + (IntPtr)3 * 4) = data[3];
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I4_3);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I4_4);
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Ldc_I4, data[2]);
			il.Emit(OpCodes.Stind_I4);

			//ReadOnlySpan<int> span = new Span<int>(ptr, 4);
			il.Emit(OpCodes.Ldc_I4_4);
			il.Emit(OpCodes.Newobj, module.ImportReference(typeof(Span<int>).GetCachedConstructor(typeof(void*), typeof(int))));
			il.Emit(OpCodes.Call, module.ImportReference(typeof(Span<int>).GetCachedImplicitOperator(typeof(ReadOnlySpan<int>))));

			//new decimal(span);
			il.Emit(OpCodes.Call, module.ImportReference(typeof(decimal).GetCachedConstructor(typeof(ReadOnlySpan<int>))));
			return true;
		}
	}
}
