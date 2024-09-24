using Chips.Common.DataStructures;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal partial struct _VariantObject : IChipsObject, IMutableInteger {
		public _VariantObjectData data;
		public _Variant type;
		public int subtype;

		public static _VariantObject GetNullObject() {
			_VariantObject obj = default;
			obj.type = _Variant.Object;
			obj.subtype = (int)_ObjectType.Reference;
			GetObject<object>(ref obj) = null;
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref T? GetObject<T>(ref _VariantObject obj) where T : class => ref _ObjectData.AsObject<T>(ref obj.data.Object);

		// Redirection to Ref<T> is necessary to ensure that the struct fits within the VariantObjectData union
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref T GetStruct<T>(ref _VariantObject obj) where T : struct => ref _ObjectData.AsStruct<T>(ref obj.data.Object);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SetStruct<T>(ref _VariantObject obj, in T value) where T : struct => _ObjectData.AsObject<Ref<T>>(ref obj.data.Object) = new Ref<T>(value);
	}

	[StructLayout(LayoutKind.Explicit, Size = sizeof(decimal))]  // Decimal and Vector<T> are both 128-bit types
	internal struct _VariantObjectData {
		[FieldOffset(0)] public _IntegerData Integer;
		[FieldOffset(0)] public _AddressData Address;
		[FieldOffset(0)] public _FloatData Float;
		[FieldOffset(0)] public _StringData String;
		[FieldOffset(0)] public _ObjectData Object;
		[FieldOffset(0)] public _VectorData Vector;
		[FieldOffset(0)] public _TokenData Token;
		[FieldOffset(0)] public _ExceptionData Exception;
		[FieldOffset(0)] public _StatusData Status;
	}

	internal enum _Variant {
		Integer,
		Address,
		Float,
		String,
		Object,
		Vector,
		Token,
		Exception,
		Status
	}
}
