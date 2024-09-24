namespace Chips.Runtime.Meta {
	partial class ObjectOperations {
		public static string? ToString_Token<TToken>(this in TypedRegister<TToken> source) where TToken : struct, IConvertToToken<TToken> {
			ref _TokenHandle sourceObj = ref TToken.AsToken(in source.value, stackalloc _TokenHandle[1]);

			return sourceObj.type switch {
				_Token.Type => sourceObj.data.TypeHandle.Value.ToString(),
				_Token.Method => sourceObj.data.MethodHandle.Value.ToString(),
				_Token.Field => sourceObj.data.FieldHandle.Value.ToString(),
				_ => throw new InvalidObjectStateException(nameof(source))
			};
		}
	}
}
