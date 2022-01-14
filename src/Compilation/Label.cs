using Chips.Core.Specifications;

namespace Chips.Compilation{
	internal class Label{
		public string name;

		public OpcodeInformation target;

		public Label(string name, OpcodeInformation target){
			this.name = name;
			this.target = target;
		}
	}
}
