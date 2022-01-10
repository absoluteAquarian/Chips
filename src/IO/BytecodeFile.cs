using Chips.Compilation;

namespace Chips.IO{
	internal class BytecodeFile{
		private readonly BinaryReader reader;

		private CPDBFile? cpdbInfo;

		private int stackSize = 1000;
		private string asmName = "CompiledChipsProgram";

		private bool stackSizeDefined, asmNameDefined;

		private HashSet<string> definedGlobalVariables = new();
		private HashSet<string> definedFunctions = new();

		public BytecodeFile(string file){
			if(Path.GetExtension(file) != ".bchp")
				throw new IOException("File extension was not \".bchp\"");

			reader = new(File.OpenRead(file));

			var cpdb = Path.ChangeExtension(file, ".cpdb");
			if(File.Exists(cpdb))
				cpdbInfo = new(cpdb);
		}

		public void Compile(){
			long fileLength = reader.BaseStream.Length;
			while(reader.BaseStream.Position < fileLength){
				BytecodeFileMember member = (BytecodeFileMember)reader.ReadByte();

				switch(member){
					case BytecodeFileMember.AssemblyInfo:
						byte asmInfo = reader.ReadByte();

						switch(asmInfo){
							case 0:
								if(asmNameDefined)
									throw new IOException("Duplicate assembly name token found");

								asmNameDefined = true;
								asmName = reader.ReadString();
								break;
							case 1:
								if(stackSizeDefined)
									throw new IOException("Duplicate stack size token found");

								stackSizeDefined = true;
								stackSize = reader.Read7BitEncodedInt();
								break;
							default:
								throw new IOException($"Unknown assembly information token: 0x{asmInfo :X2}");
						}
						break;
					case BytecodeFileMember.GlobalVariable:

						break;
					case BytecodeFileMember.Function:

						break;
				}
			}

			ConstructEntryPoint();
		}

		private void ConstructEntryPoint(){
			//Creates the static Main() method, which invokes Chips.Core.Sandbox.Main()
		}
	}
}
