using System;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		public void Add(Register target, Register source, Register operand) {
			_RegisterType sourceType = RegisterToClassification(source);
			_RegisterType operandType = RegisterToClassification(operand);
			_RegisterType targetType = RegisterToClassification(target);

			switch (targetType) {
				case _RegisterType.Address:
					GetAddress(target).value = sourceType switch {
						_RegisterType.Address => operandType switch {
							_RegisterType.Address => Add_Address_Address(source, operand),
							_RegisterType.Integer => Add_Address_Integer(source, operand),
							_RegisterType.Variant => GetVariantObject(operand).RequireAddressOrInteger().Add_VariantObj_Address(GetAddress(source)).UnwrapAddress(),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Integer => operandType switch {
							_RegisterType.Address => Add_Address_Integer(operand, source),
							_RegisterType.Integer => throw new InvalidTargetException(target, source, operand),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_ => throw new InvalidTargetException(target, source)
					};
					break;
				case _RegisterType.Integer:
					GetInteger(target).value = sourceType switch {
						_RegisterType.Integer => operandType switch {
							_RegisterType.Integer => Add_Integer_Integer(source, operand),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_ => throw new InvalidTargetException(target, source)
					};
					break;
				case _RegisterType.Float:
					GetFloat(target).value = sourceType switch {
						_RegisterType.Integer => operandType switch {
							_RegisterType.Float => Add_Float_Integer(operand, source),
							_RegisterType.Integer => throw new InvalidTargetException(target, source, operand),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Float => operandType switch {
							_RegisterType.Float => Add_Float_Float(source, operand),
							_RegisterType.Integer => Add_Float_Integer(source, operand),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_ => throw new InvalidTargetException(target, source)
					};
					break;
				case _RegisterType.String:
					GetStringObject(target).value = sourceType switch {
						_RegisterType.Address => operandType switch {
							_RegisterType.String => GetAddress(source).ToString_AddressObj().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Integer => operandType switch {
							_RegisterType.String => GetInteger(source).ToString_NumberInteger().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Float => operandType switch {
							_RegisterType.String => GetFloat(source).ToString_NumberFloat().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.String => operandType switch {
							_RegisterType.String => Add_String_String(source, operand),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Object => operandType switch {
							_RegisterType.String => GetObject(source).ToString_Object().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Vector => operandType switch {
							_RegisterType.String => GetVectorObject(source).ToString_VectorObj().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Token => operandType switch {
							_RegisterType.String => GetToken(source).ToString_Token().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Variant => operandType switch {
							_RegisterType.Address => GetVariantObject(source).RequireString().Add_VariantObj_Address(GetAddress(operand)).UnwrapString(),
							_RegisterType.Integer => GetVariantObject(source).RequireString().Add_VariantObj_NumberInteger(GetInteger(operand)).UnwrapString(),
							_RegisterType.Float => GetVariantObject(source).RequireString().Add_VariantObj_NumberFloat(GetFloat(operand)).UnwrapString(),
							_RegisterType.String => GetVariantObject(source).Add_VariantObj_StringObj(GetStringObject(operand)).UnwrapString(),
							_RegisterType.Object => GetVariantObject(source).RequireString().Add_StringObj_StringVal(GetObject(operand).ToString_Object()),
							_RegisterType.Vector => GetVariantObject(source).RequireString().Add_VariantObj_VectorObj(GetVectorObject(operand)).UnwrapString(),
							_RegisterType.Token => GetVariantObject(source).RequireString().Add_StringObj_StringVal(GetToken(operand).ToString_Token()),
							_RegisterType.Variant => Add_Variant_Variant(source, operand).RequireString().UnwrapString(),
							_RegisterType.Exception => GetVariantObject(source).RequireString().Add_StringObj_StringVal(GetException(operand).ToString_Exception()),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Exception => operandType switch {
							_RegisterType.String => GetException(source).ToString_Exception().Add_StringVal_StringObj(GetStringObject(operand)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_ => throw new InvalidTargetException(target, source)
					};
					break;
				case _RegisterType.Object:
					// GetObject(target).value =
					throw new NotImplementedException("Implementation for Object not found");  // TODO: Implement Object arithmetic
				case _RegisterType.Vector:
					GetVectorObject(target).value = sourceType switch {
						_RegisterType.Vector => operandType switch {
							_RegisterType.Vector => GetVectorObject(source).Add_VectorObj_VectorObj(GetVectorObject(operand)),
							_RegisterType.Variant => GetVariantObject(operand).Add_VariantObj_VectorObj(GetVectorObject(source)).RequireVector().UnwrapVector(),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Variant => operandType switch {
							_RegisterType.Vector => GetVariantObject(source).Add_VariantObj_VectorObj(GetVectorObject(operand)).RequireVector().UnwrapVector(),
							_RegisterType.Variant => Add_Variant_Variant(source, operand).RequireVector().UnwrapVector(),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_ => throw new InvalidTargetException(target, source)
					};
					break;
				case _RegisterType.Variant:
					GetVariantObject(target).value = sourceType switch {
						_RegisterType.Address => operandType switch {
							_RegisterType.Address => Add_Address_Address(source, operand).AsVariant(),
							_RegisterType.Integer => Add_Address_Integer(source, operand).AsVariant(),
							_RegisterType.String => GetAddress(source).ToString_AddressObj().Add_StringVal_StringObj(GetStringObject(operand)).AsVariant(),
							_RegisterType.Variant => GetVariantObject(operand).Add_VariantObj_Address(GetAddress(source)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Integer => operandType switch {
							_RegisterType.Address => Add_Address_Integer(operand, source).AsVariant(),
							_RegisterType.Integer => Add_Integer_Integer(source, operand).AsVariant(),
							_RegisterType.Float => Add_Float_Integer(operand, source).AsVariant(),
							_RegisterType.String => GetInteger(source).ToString_NumberInteger().Add_StringVal_StringObj(GetStringObject(operand)).AsVariant(),
							_RegisterType.Variant => GetVariantObject(operand).Add_VariantObj_NumberInteger(GetInteger(source)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Float => operandType switch {
							_RegisterType.Integer => Add_Float_Integer(source, operand).AsVariant(),
							_RegisterType.Float => Add_Float_Float(source, operand).AsVariant(),
							_RegisterType.String => GetFloat(source).ToString_NumberFloat().Add_StringVal_StringObj(GetStringObject(operand)).AsVariant(),
							_RegisterType.Variant => GetVariantObject(operand).Add_VariantObj_NumberFloat(GetFloat(source)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.String => operandType switch {
							_RegisterType.Address => GetAddress(operand).ToString_AddressObj().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_RegisterType.Integer => GetInteger(operand).ToString_NumberInteger().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_RegisterType.Float => GetFloat(operand).ToString_NumberFloat().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_RegisterType.String => Add_String_String(source, operand).AsVariant(),
							_RegisterType.Object => GetObject(operand).ToString_Object().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_RegisterType.Vector => GetVectorObject(operand).ToString_VectorObj().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_RegisterType.Token => GetToken(operand).ToString_Token().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_RegisterType.Variant => GetStringObject(source).Add_StringObj_VariantObj(GetVariantObject(operand)),
							_RegisterType.Exception => GetException(operand).ToString_Exception().Add_StringVal_StringObj(GetStringObject(source)).AsVariant(),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Object => throw new NotImplementedException("Variant arithmetic with Object not implemented"),
						_RegisterType.Vector => operandType switch {
							_RegisterType.String => GetVectorObject(source).ToString_VectorObj().Add_StringVal_StringObj(GetStringObject(operand)).AsVariant(),
							_RegisterType.Vector => GetVectorObject(source).Add_VectorObj_VectorObj(GetVectorObject(operand)).AsVariant(),
							_RegisterType.Variant => GetVariantObject(operand).Add_VariantObj_VectorObj(GetVectorObject(source)),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_RegisterType.Variant => operandType switch {
							_RegisterType.Address => GetVariantObject(source).Add_VariantObj_Address(GetAddress(operand)),
							_RegisterType.Integer => GetVariantObject(source).Add_VariantObj_NumberInteger(GetInteger(operand)),
							_RegisterType.Float => GetVariantObject(source).Add_VariantObj_NumberFloat(GetFloat(operand)),
							_RegisterType.String => GetVariantObject(source).Add_VariantObj_StringObj(GetStringObject(operand)),
							_RegisterType.Object => throw new NotImplementedException("Variant arithmetic with Object not implemented"),
							_RegisterType.Vector => GetVariantObject(source).Add_VariantObj_VectorObj(GetVectorObject(operand)),
							_RegisterType.Variant => Add_Variant_Variant(source, operand),
							_ => throw new InvalidTargetException(target, source, operand)
						},
						_ => throw new InvalidTargetException(target, source)
					};
					break;
				case _RegisterType.Token:
				case _RegisterType.Exception:
				case _RegisterType.Status:
					throw new InvalidTargetException(target);
				default:
					throw new ArgumentException("Invalid target register type");
			}
		}
	}
}
