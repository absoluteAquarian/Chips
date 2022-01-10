using Chips.Core.Specifications;
using System.Text;

namespace Chips{
	public enum CompilationContext{
		NotCompiling = 0,

		/// <summary>
		/// ".asm_name", ".stacksize" and global variables
		/// </summary>
		CompilingGlobalInformation,

		/// <summary>
		/// A global function
		/// </summary>
		CompilingGlobalMethod
	}

	public static class Compiler{
		public static Dictionary<string, bool> BytecodeOptions = new(){
			["inline"] = false,
			["no-source"] = false,
			["unsafe"] = false,
			["allow-stack-overflow"] = false
		};

		public static readonly string[] opcodeNames;

		static Compiler(){
			int index;
			opcodeNames = typeof(Opcodes).GetFields().Where(f => f.FieldType == typeof(Opcode))
				.Select(f => ((index = f.Name.IndexOf('_')) >= 0 ? f.Name[..index] : f.Name).ToLower())
				.Distinct()
				.ToArray();
		}

		public static void Main(string[] args){
			
		}

		public static bool OpcodeIsDefined(string code){
			for(int i = 0; i < opcodeNames.Length; i++)
				if(opcodeNames[i] == code)
					return true;

			return false;
		}
	}
}