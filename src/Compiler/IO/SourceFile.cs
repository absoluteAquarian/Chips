using Chips.Compilation;

namespace Chips.Compiler.IO {
	internal class SourceFile {
		public readonly string file;

		public SourceFile(string file) {
			this.file = file;
		}

		public BytecodeFile CompileToBytecode(ModuleInformationTree module) {
			
		}
	}
}
