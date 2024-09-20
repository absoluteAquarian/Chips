namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private struct _NumberInteger {
			public _IntegerData data;
			public _Integer type;
		}

		private struct _Address {
			public _AddressData data;
			public _NativeInt type;
		}

		private struct _NumberFloat {
			public _FloatData data;
			public _Float type;
		}

		private struct _StringObject {
			public object data;
			public _String type;
		}

		private struct _VariantObject {
			public _VariantObjectData data;
			public _Variant type;
			public int subtype;
		}

		private struct _VectorObject {
			public _VectorData data;
			public _Vector type;
		}

		private struct _MetadataTokenHandle {
			public _MetadataTokenData data;
			public _MetadataToken type;
		}
	}
}
