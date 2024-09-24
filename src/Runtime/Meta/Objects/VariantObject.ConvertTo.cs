using Chips.Common.DataStructures;
using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	partial struct _VariantObject : IConvertToInteger<_VariantObject>, IConvertToAddress<_VariantObject>, IConvertToFloat<_VariantObject>, IConvertToString<_VariantObject>, IConvertToObject<_VariantObject>, IConvertToVector<_VariantObject>, IConvertToToken<_VariantObject>, IConvertToException<_VariantObject>, IConvertToStatus<_VariantObject> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _NumberInteger IConvertToInteger<_VariantObject>.AsInteger(in _VariantObject obj, Span<_NumberInteger> stackAlloc1) {
			ref _NumberInteger result = ref stackAlloc1[0];
			result.data = obj.data.Integer;
			result.type = (_Integer)obj.subtype;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _Address IConvertToAddress<_VariantObject>.AsAddress(in _VariantObject obj, Span<_Address> stackAlloc1) {
			ref _Address result = ref stackAlloc1[0];
			result.data = obj.data.Address;
			result.type = (_NativeInt)obj.subtype;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _NumberFloat IConvertToFloat<_VariantObject>.AsFloat(in _VariantObject obj, Span<_NumberFloat> stackAlloc1) {
			ref _NumberFloat result = ref stackAlloc1[0];
			result.data = obj.data.Float;
			result.type = (_Float)obj.subtype;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _StringObject IConvertToString<_VariantObject>.AsString(in _VariantObject obj, Span<_StringObject> stackAlloc1) {
			ref _StringObject result = ref stackAlloc1[0];
			result.data = obj.data.String;
			result.type = (_String)obj.subtype;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _Object IConvertToObject<_VariantObject>.AsObject(in _VariantObject obj, Span<_Object> stackAlloc1) {
			ref _Object result = ref stackAlloc1[0];
			result.type = (_ObjectType)obj.subtype;

			ref object? objData = ref GetObject<object>(ref Unsafe.AsRef(in obj));
			if (objData is __Ref objRef)
				_ObjectData.AsObject<object>(ref result.data) = objRef.Copy();  // Ref<T>.Copy() makes a shallow copy of its value if T is a valuetype
			else
				_ObjectData.AsObject<object>(ref result.data) = objData;

			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _VectorObject IConvertToVector<_VariantObject>.AsVector(in _VariantObject obj, Span<_VectorObject> stackAlloc1) {
			ref _VectorObject result = ref stackAlloc1[0];
			result.data = obj.data.Vector;
			result.type = (_Vector)obj.subtype;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _TokenHandle IConvertToToken<_VariantObject>.AsToken(in _VariantObject obj, Span<_TokenHandle> stackAlloc1) {
			ref _TokenHandle result = ref stackAlloc1[0];
			result.data = obj.data.Token;
			result.type = (_Token)obj.subtype;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _ExceptionObject IConvertToException<_VariantObject>.AsException(in _VariantObject value, Span<_ExceptionObject> stackAlloc1) {
			ref _ExceptionObject result = ref stackAlloc1[0];
			result.data = value.data.Exception;
			return ref result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ref _StatusObject IConvertToStatus<_VariantObject>.AsStatus(in _VariantObject value, Span<_StatusObject> stackAlloc1) {
			ref _StatusObject result = ref stackAlloc1[0];
			result.data = value.data.Status;
			return ref result;
		}
	}
}
