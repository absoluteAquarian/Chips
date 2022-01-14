using Chips.Core.Meta;
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
		/// <summary>
		/// The compiler flags
		/// <list type="bullet">
		/// <item>inline: Short-length opcodes (e.g. <seealso cref="Opcodes.Clo"/>) are inserted directly into the compiled method</item>
		/// <item>no-source: <seealso cref="Opcode.FunctionContext"/> instances are not passed to opcode calls</item>
		/// <item>unsafe: Type checking is not performed, most opcodes are inlined and no function context is provided (overrides "inline" and "no-source")</item>
		/// <item>allow-stack-overflow: <seealso cref="Metadata.Registers.SP"/> can overflow to the other end of the <seealso cref="Metadata.stack"/></item>
		/// </list>
		/// </summary>
		public static readonly Dictionary<string, bool> BytecodeOptions = new(){
			["inline"] = false,
			["no-source"] = false,
			["unsafe"] = false,
			["allow-stack-overflow"] = false
		};

		public static readonly string[] opcodeNames;

		public static readonly List<CompilationException> exceptions = new();

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