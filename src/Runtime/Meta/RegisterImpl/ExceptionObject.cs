namespace Chips.Runtime.Meta {
	partial class ObjectOperations {
		public static string? ToString_Exception<T>(this in TypedRegister<T> source) where T : struct, IConvertToException<T> {
			ref _ExceptionObject obj = ref T.AsException(in source.value, stackalloc _ExceptionObject[1]);
			return _ExceptionData.Get(ref obj.data)?.ToString();
		}
	}
}
