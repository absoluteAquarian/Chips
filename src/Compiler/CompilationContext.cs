using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using Chips.Compiler.Compilation;
using Chips.Compiler.Utility;

namespace Chips.Compiler {
	public sealed class CompilationContext {
		public readonly ReferenceImporter importer;
		public readonly TypeResolver resolver;
		public readonly StringHeap heap;

		public CILCursor Cursor { get; private set; }

		public BytecodeMethodBody ActiveMethod { get; private set; }

		internal CompilationContext(ReferenceImporter importer) {
			this.importer = importer;
			resolver = new();
			heap = new();
		}

		public void SetCILMethod(CilMethodBody method) {
			if (method is not null) {
				Cursor = new CILCursor(method);
				ActiveMethod = null!;
			} else
				Cursor = null!;
		}

		public void SetChipsMethod(BytecodeMethodBody method) {
			if (method is not null) {
				Cursor = null!;
				ActiveMethod = method;
			} else
				ActiveMethod = null!;
		}
	}
}
