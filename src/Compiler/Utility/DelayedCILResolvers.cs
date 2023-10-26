using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;

namespace Chips.Compiler.Utility {
	public interface IDelayedCILMetadataResolver {
		void Resolve(CompilationContext context);
	}

	public sealed class DelayedLocalTypeResolver : IDelayedCILMetadataResolver {
		public readonly CilMethodBody body;
		public readonly int localIndex;
		public readonly ITypeDefOrRef type;

		public DelayedLocalTypeResolver(CilMethodBody body, int localIndex, ITypeDefOrRef type) {
			this.body = body;
			this.localIndex = localIndex;
			this.type = type;
		}

		public void Resolve(CompilationContext context) {
			body.LocalVariables[localIndex].VariableType = context.importer.ImportTypeSignature(type.ToTypeSignature());
		}
	}
}
