using Chips.Common.DataStructures;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _Object : IChipsObject, ICreateObject<_Object>, IConvertToObject<_Object> {
		public _ObjectData data;
		public _ObjectType type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _Object IConvertToObject<_Object>.AsObject(in _Object value, Span<_Object> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _Object ICreateObject<_Object>.Create<U>(in U value) {
			if (typeof(U) == typeof(_Object)) {
				// Ensure valuetype copying
				ref _Object orig = ref Unsafe.As<U, _Object>(ref Unsafe.AsRef(in value));
				_Object copy = default;
				copy.type = orig.type;

				if (orig.type == _ObjectType.Valuetype)
					_ObjectData.AsObject<object>(ref copy.data) = ((__Ref)_ObjectData.AsObject<object>(ref orig.data)!).Copy();
				else
					_ObjectData.AsObject<object>(ref copy.data) = _ObjectData.AsObject<object>(ref orig.data);

				return copy;
			}

			_Object obj = default;
			obj.type = TypeMetrics.TypeToObject<U>();

			if (TypeMetrics.TypeToObject<U>() == _ObjectType.Valuetype)
				_ObjectData.AsObject<object>(ref obj.data) = new Ref<U>(value);
			else
				_ObjectData.AsObject<object>(ref obj.data) = value;

			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
	internal struct _ObjectData {
		[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
		private struct _Intrinsic { }

		[FieldOffset(0)] private _Intrinsic Object;

		// "object" cannot be at the same FieldOffset as non-reference types, hence the redirection
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref T? AsObject<T>(ref _ObjectData data) where T : class => ref Unsafe.As<_Intrinsic, T?>(ref data.Object);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref T AsStruct<T>(ref _ObjectData data) where T : struct {
			ref Ref<T> objRef = ref Unsafe.As<_Intrinsic, Ref<T>>(ref data.Object);
			objRef ??= new Ref<T>(default);
			return ref objRef.value;
		}
	}

	internal enum _ObjectType {
		Reference,
		Valuetype
	}
}
