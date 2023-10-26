using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using Chips.Compiler.Utility;

namespace Chips.Compiler {
	public sealed class CompilationContext {
		public readonly ReferenceImporter importer;
		public readonly TypeResolver resolver;
		public readonly StringHeap heap;

		public CILCursor Cursor { get; private set; }

		internal CompilationContext(ReferenceImporter importer) {
			this.importer = importer;
			resolver = new();
			heap = new();
		}

		public void SetMethod(CilMethodBody method) {
			if (method is not null)
				Cursor = new CILCursor(method);
			else
				Cursor = null!;
		}
	}
}
