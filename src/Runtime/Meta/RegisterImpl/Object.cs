using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	partial class ObjectOperations {
		public static string? ToString_Object<TObject>(this in TypedRegister<TObject> source) where TObject : struct, IConvertToObject<TObject> {
			// Special case for hot path
			if (typeof(TObject) == typeof(_VariantObject))
				return _VariantObject.GetObject<object>(ref Unsafe.As<TObject, _VariantObject>(ref source.value))?.ToString();

			ref _Object sourceObj = ref TObject.AsObject(in source.value, stackalloc _Object[1]);
			return _ObjectData.AsObject<object>(ref sourceObj.data)?.ToString();
		}
	}
}
