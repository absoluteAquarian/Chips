namespace Chips.Compilation {
	internal enum OperandInformationType {
		Constant = 1,
		Variable,
		Label,
		TypeString,
		TypeCode,
		FunctionCall,
		CollectionAccessIndexByX,
		CollectionAccessIndexByY
	}
}
