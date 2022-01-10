namespace Chips.Compilation{
	internal abstract class OpcodeArgument{
		public readonly OperandInformationType type;

		private byte[] data;

		public ReadOnlySpan<byte> Data => new(data);

		public OpcodeArgument(byte[] data, OperandInformationType type){
			this.data = data;
			this.type = type;
		}
	}

	internal class OpcodeArgumentConstant : OpcodeArgument{
		public readonly OperandConstantType valueType;

		public ReadOnlySpan<byte> ValueData => Data[1..];

		public OpcodeArgumentConstant(byte[] data) : base(data, OperandInformationType.Constant){
			valueType = (OperandConstantType)data[0];

			switch(valueType){
				case OperandConstantType.BigInt:
				case OperandConstantType.Object:
				case OperandConstantType.Array:
				case OperandConstantType.List:
				case OperandConstantType.Time:
				case OperandConstantType.Set:
				case OperandConstantType.Date:
				case OperandConstantType.Regex:
				case OperandConstantType.Random:
				case OperandConstantType.UserDef:
					throw new IOException($"Constant type {valueType} cannot be used as a constant");
			}
		}
	}

	// TODO: the other argument types
}
