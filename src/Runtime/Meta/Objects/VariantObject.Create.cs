using Chips.Common.DataStructures;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	partial struct _VariantObject : ICreateObject<_VariantObject> {
		static _VariantObject ICreateObject<_VariantObject>.Create<U>(in U value) {
			_VariantObject result = default;

			if (typeof(U) == typeof(_NumberInteger)) {
				ref _NumberInteger obj = ref Unsafe.As<U, _NumberInteger>(ref Unsafe.AsRef(in value));
				result.data.Integer = obj.data;
				result.type = _Variant.Integer;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_Address)) {
				ref _Address obj = ref Unsafe.As<U, _Address>(ref Unsafe.AsRef(in value));
				result.data.Address = obj.data;
				result.type = _Variant.Address;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_NumberFloat)) {
				ref _NumberFloat obj = ref Unsafe.As<U, _NumberFloat>(ref Unsafe.AsRef(in value));
				result.data.Float = obj.data;
				result.type = _Variant.Float;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_StringObject)) {
				ref _StringObject obj = ref Unsafe.As<U, _StringObject>(ref Unsafe.AsRef(in value));

				ref object? objData = ref _StringData.AsObject(ref obj.data);
				if (objData is null)
					return GetNullObject();

				result.data.String = obj.data;
				result.type = _Variant.String;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_Object)) {
				ref _Object obj = ref Unsafe.As<U, _Object>(ref Unsafe.AsRef(in value));

				ref object? objData = ref _ObjectData.AsObject<object>(ref obj.data);
				if (objData is null)
					return GetNullObject();
				else if (objData is __Ref objRef)
					GetObject<object>(ref result) = objRef.Copy();  // Ref<T>.Copy() makes a shallow copy of its value if T is a valuetype
				else
					GetObject<object>(ref result) = objData;

				result.type = _Variant.Object;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_VectorObject)) {
				ref _VectorObject obj = ref Unsafe.As<U, _VectorObject>(ref Unsafe.AsRef(in value));
				result.data.Vector = obj.data;
				result.type = _Variant.Vector;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_TokenHandle)) {
				ref _TokenHandle obj = ref Unsafe.As<U, _TokenHandle>(ref Unsafe.AsRef(in value));
				result.data.Token = obj.data;
				result.type = _Variant.Token;
				result.subtype = (int)obj.type;
			} else if (typeof(U) == typeof(_ExceptionObject)) {
				ref _ExceptionObject obj = ref Unsafe.As<U, _ExceptionObject>(ref Unsafe.AsRef(in value));
				result.data.Exception = obj.data;
				result.type = _Variant.Exception;
				result.subtype = -1;
			} else if (typeof(U) == typeof(_StatusObject)) {
				ref _StatusObject obj = ref Unsafe.As<U, _StatusObject>(ref Unsafe.AsRef(in value));
				result.data.Status = obj.data;
				result.type = _Variant.Status;
				result.subtype = -1;
			} else if (typeof(U) == typeof(_VariantObject)) {
				ref _VariantObject obj = ref Unsafe.As<U, _VariantObject>(ref Unsafe.AsRef(in value));
				result.type = obj.type;
				result.subtype = obj.subtype;

				switch (obj.type) {
					case _Variant.Integer: result.data.Integer = obj.data.Integer; break;
					case _Variant.Address: result.data.Address = obj.data.Address; break;
					case _Variant.Float: result.data.Float = obj.data.Float; break;
					case _Variant.String: result.data.String = obj.data.String; break;
					case _Variant.Object:
						ref object? objData = ref GetObject<object>(ref obj);

						if (objData is null)
							return GetNullObject();
						else if (objData is __Ref objRef)
							GetObject<object>(ref result) = objRef.Copy();
						else
							GetObject<object>(ref result) = objData;
						break;
					case _Variant.Vector: result.data.Vector = obj.data.Vector; break;
					case _Variant.Token: result.data.Token = obj.data.Token; break;
					case _Variant.Exception: result.data.Exception = obj.data.Exception; break;
					case _Variant.Status: result.data.Status = obj.data.Status; break;
					default: throw new InvalidObjectStateException(nameof(value));
				}
			} else {
				if (value is null) {
					// Treat it as a null object
					return GetNullObject();
				}

				result.type = TypeMetrics.TypeToVariant<U>();
				result.subtype = TypeMetrics.TypeToVariantSubtype<U>();

				if (TypeMetrics.TypeToVariant<U>() == _Variant.Object) {
					// SetStruct<U>() and GetObject<U>() are not available due to their constraints
					if (TypeMetrics.TypeToVariantSubtype<U>() == (int)_ObjectType.Valuetype)
						GetObject<object>(ref result) = new Ref<U>(value);
					else
						GetObject<object>(ref result) = value;
				} else
					Unsafe.As<_VariantObject, U>(ref result) = value;
			}

			return result;
		}
	}
}
