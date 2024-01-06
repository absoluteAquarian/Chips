using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using Chips.Runtime;

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

	public sealed class DelayedGenericImplMethodResolver : IDelayedCILMetadataResolver {
		public readonly CilMethodBody body;
		public readonly int instructionIndex;
		public readonly ITypeDefOrRef type;
		public readonly string implMethod;

		public DelayedGenericImplMethodResolver(CilMethodBody body, int instructionIndex, ITypeDefOrRef type, string implMethod) {
			this.body = body;
			this.instructionIndex = instructionIndex;
			this.type = type;
			this.implMethod = implMethod;
		}

		public void Resolve(CompilationContext context) {
			var instruction = body.Instructions[instructionIndex];

			var importedMethod = (IMethodDescriptor)context.importer.ImportMethod(typeof(Implementation).GetMethod(implMethod)!)
				.Resolve()!.MakeGenericInstanceMethod(type.ToTypeSignature());

			instruction.ReplaceWith(CilOpCodes.Call, importedMethod);
		}
	}
}
