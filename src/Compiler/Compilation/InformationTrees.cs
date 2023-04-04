using Mono.Cecil;
using System.Collections.Generic;

namespace Chips.Compilation {
	internal class AssemblyInformationTree : Dictionary<string, ModuleInformationTree> {
		public readonly AssemblyDefinition Assembly;

		public AssemblyInformationTree(AssemblyDefinition assembly) : base() {
			Assembly = assembly;
		}
	}

	internal class ModuleInformationTree : Dictionary<string, TypeInformationTree> {
		public readonly ModuleDefinition Module;

		public ModuleInformationTree(ModuleDefinition module) : base() {
			Module = module;
		}
	}

	internal class TypeInformationTree {
		private readonly Dictionary<string, FieldDefinition> globals = new();
		private readonly Dictionary<string, MethodDefinition> functions = new();

		public readonly TypeDefinition Type;

		public TypeInformationTree(TypeDefinition type) {
			Type = type;
		}

		public void Add(string name, FieldDefinition global)
			=> globals.Add(name, global);

		public void Add(string name, MethodDefinition function)
			=> functions.Add(name, function);

		public FieldDefinition GetGlobal(string name)
			=> globals[name];

		public MethodDefinition GetFunction(string name)
			=> functions[name];

		public bool TryGetGlobal(string name, out FieldDefinition? global)
			=> globals.TryGetValue(name, out global);

		public bool TryGetFunction(string name, out MethodDefinition? function)
			=> functions.TryGetValue(name, out function);
	}
}
