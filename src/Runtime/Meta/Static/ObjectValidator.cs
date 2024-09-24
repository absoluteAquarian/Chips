using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	internal static class ObjectValidator {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireInteger(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Integer)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireInteger(this in _VariantObject obj) {
			if (obj.type != _Variant.Integer)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Integer);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireAddress(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Address)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireAddress(this in _VariantObject obj) {
			if (obj.type != _Variant.Address)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Address);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireAddressOrInteger(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Address && obj.value.type != _Variant.Integer)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireAddressOrInteger(this in _VariantObject obj) {
			if (obj.type != _Variant.Address && obj.type != _Variant.Integer)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Address, _Variant.Integer);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireFloat(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Float)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireFloat(this in _VariantObject obj) {
			if (obj.type != _Variant.Float)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Float);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireNumber(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Integer && obj.value.type != _Variant.Float)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireNumber(this in _VariantObject obj) {
			if (obj.type != _Variant.Integer && obj.type != _Variant.Float)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Integer, _Variant.Float);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireString(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.String)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireString(this in _VariantObject obj) {
			if (obj.type != _Variant.String)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.String);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireObject(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Object)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireObject(this in _VariantObject obj) {
			if (obj.type != _Variant.Object)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Object);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireVector(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Vector)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireVector(this in _VariantObject obj) {
			if (obj.type != _Variant.Vector)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Vector);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireToken(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Token)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireToken(this in _VariantObject obj) {
			if (obj.type != _Variant.Token)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Token);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireException(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Exception)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireException(this in _VariantObject obj) {
			if (obj.type != _Variant.Exception)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Exception);
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TypedRegister<_VariantObject> RequireStatus(this in TypedRegister<_VariantObject> obj) {
			if (obj.value.type != _Variant.Status)
				throw obj.InvalidType();
			return ref obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject RequireStatus(this in _VariantObject obj) {
			if (obj.type != _Variant.Status)
				throw new InvalidVariantObjectTypeException(in obj, _Variant.Status);
			return obj;
		}
	}
}
