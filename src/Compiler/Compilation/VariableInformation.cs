namespace Chips.Compilation {
	internal class VariableInformation {
		public readonly bool global;
		public readonly string name;
		public readonly string type;

		public VariableInformation(bool global, string name, string type) {
			this.global = global;
			this.name = name;
			this.type = type;
		}
	}
}
