using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	internal struct _StatusObject : IChipsObject, ICreateObject<_StatusObject>, IConvertToInteger<_StatusObject>, IConvertToStatus<_StatusObject> {
		public _StatusData data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref readonly ushort GetWord(in _StatusObject obj) => ref _StatusData.GetWord(ref Unsafe.AsRef(in obj.data));

		static ref _NumberInteger IConvertToInteger<_StatusObject>.AsInteger(in _StatusObject value, Span<_NumberInteger> stackAlloc1) {
			ref _NumberInteger result = ref stackAlloc1[0];
			result.data.UInt16 = GetWord(in value);
			result.type = _Integer.UInt16;
			return ref result;
		}

		static ref _StatusObject IConvertToStatus<_StatusObject>.AsStatus(in _StatusObject value, Span<_StatusObject> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _StatusObject ICreateObject<_StatusObject>.Create<U>(in U value) {
			if (typeof(U) == typeof(_StatusObject))
				return Unsafe.As<U, _StatusObject>(ref Unsafe.AsRef(in value));
			else if (typeof(U) != typeof(Status))
				throw new InvalidConversionException<U>("StatusObject");

			_StatusObject obj = default;
			obj.data.Status = Unsafe.As<U, Status>(ref Unsafe.AsRef(in value));
			return obj;
		}
	}

	// Only included for the helper method
	internal struct _StatusData {
		public Status Status;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref readonly ushort GetWord(ref _StatusData data) => ref Unsafe.As<_StatusData, ushort>(ref data);
	}
}
