using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using Chips.Compiler.Utility;

namespace Chips.Compiler {
	public sealed class CompilationContext {
		public readonly ReferenceImporter importer;
		private CilMethodBody _method;
		public readonly TypeResolver resolver;
		public readonly StringHeap heap;
		public readonly LooseEvaluationStackSimulator stack;

		public CilMethodBody Body => _method;

		public CilInstructionCollection Instructions => _method?.Instructions!;

		internal CompilationContext(ReferenceImporter importer) {
			this.importer = importer;
			resolver = new();
			heap = new();
			stack = new();
		}

		public void SetMethod(CilMethodBody method) {
			_method = method;
			stack.Clear();
		}
	}
}
