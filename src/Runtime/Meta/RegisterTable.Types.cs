using Chips.Common.DataStructures;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private struct _NumberInteger {
			public readonly _IntegerData data;
			public _Integer type;
		}

		private struct _Address {
			public readonly _AddressData data;
			public _NativeInt type;
		}

		private struct _NumberFloat {
			public readonly _FloatData data;
			public _Float type;
		}

		private struct _StringObject {
			public readonly object data;
			public _String type;
		}

		private struct _VariantObject {
			public readonly _VariantObjectData data;
			public _Variant type;
			public int subtype;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ref T? GetObject<T>(ref _VariantObject obj) where T : class => ref Unsafe.As<_VariantObject, T?>(ref obj);

			// Redirection to Ref<T> is necessary to ensure that the struct fits within the VariantObjectData union
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void SetStruct<T>(ref _VariantObject obj, in T value) where T : struct => Unsafe.As<_VariantObject, Ref<T>>(ref obj) = new Ref<T>(value);
		}

		private struct _VectorObject {
			public readonly _VectorData data;
			public _Vector type;
		}

		private struct _MetadataTokenHandle {
			public readonly _MetadataTokenData data;
			public _MetadataToken type;
		}
	}
}
