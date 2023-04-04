using Chips.Runtime.Specifications;
using Chips.Runtime.Utility;
using Chips.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compilation {
	internal class BytecodeFunction {
		public readonly string name;

		public readonly MethodAttributes attributes;

		public List<VariableInformation> locals = new();
		public List<OpcodeInformation> instructions = new();
		public List<Label> labels = new();

		public readonly BytecodeFile file;

		public readonly int SizeInBytes;

		internal static Opcode CompilingOpcode { get; private set; }

		public BytecodeFunction(BytecodeFile file, BinaryReader reader) {
			this.file = file;

			long position = reader.BaseStream.Position;

			name = reader.ReadString();

			if (!file.definedGlobalFunctions.Add(name)) {
				ChipsCompiler.exceptions.Add(new(file.SourceFile, "Duplicate function declaration: " + name));
				return;
			}

			attributes = (MethodAttributes)reader.ReadUInt16();

			//Read locals, then opcodes, then labels
			ushort localsCount = reader.ReadUInt16();

			bool hasCPDB = file.cpdbInfo is not null;

			for (int i = 0; i < localsCount; i++) {
				ushort index = reader.ReadUInt16();

				string name = hasCPDB && file.cpdbInfo!.TryFindLocalVariableByIndex(this.name, index, out var local) ? local!.variableName : "local_" + i;
				string type = reader.ReadString();

				locals.Add(new(global: false, name, type));
			}

			int opcodeCount = reader.ReadInt32();
			int opcodeStreamLength = reader.Read7BitEncodedInt();

			byte[] bytes = reader.ReadBytes(opcodeStreamLength);
			int bytesRead = 0;

			for (int i = 0; i < opcodeCount; i++)
				instructions.Add(new(bytes, ref bytesRead));

			int labelCount = reader.ReadInt32();

			for (int i = 0; i < labelCount; i++) {
				int index = reader.ReadInt32();
				int target = reader.ReadInt32();

				string name = hasCPDB && file.cpdbInfo!.TryFindFunctionLabelByIndex(this.name, index, out var label) ? label!.labelName : $"CHP_{target:X}";

				labels.Add(new(name, instructions[target]));
			}

			SizeInBytes = (int)(reader.BaseStream.Position - position);
		}

		public bool HasLabel(string name, out int labelIndex) {
			for (int i = 0; i < labels.Count; i++) {
				if (labels[i].name == name) {
					labelIndex = i;
					return true;
				}
			}

			labelIndex = -1;
			return false;
		}

		public bool HasLocal(string name, out int localIndex) {
			for (int i = 0; i < labels.Count; i++) {
				if (locals[i].name == name) {
					localIndex = i;
					return true;
				}
			}

			localIndex = -1;
			return false;
		}

		internal void Compile(ModuleDefinition module, TypeDefinition type, MethodDefinition method) {
			var body = method.Body = new MethodBody(method) {
				InitLocals = true
			};

			var il = body.GetILProcessor();

			//Declare locals
			for (int i = 0; i < locals.Count; i++) {
				var local = locals[i];

				var variable = new VariableDefinition(module.ImportReference(TypeTracking.GetCSharpType(local.type)));
				body.Variables.Add(variable);
			}

			bool optionUnsafe = ChipsCompiler.BytecodeOptions["unsafe"];
			bool optionInline = !optionUnsafe && ChipsCompiler.BytecodeOptions["inline"];

			//Define instructions
			for (int i = 0; i < instructions.Count; i++) {
				CompilingOpcode = instructions[i].opcode;

				if (optionUnsafe)
					CompileOpcodeUnsafe(il, instructions[i]);
				else if (optionInline)
					CompileOpcodeInline(il, instructions[i]);
				else
					CompileOpcodeVerbose(il, instructions[i]);
			}

			//Define labels

			//Define exception stuff
		}

		private static readonly Dictionary<Opcode, System.Reflection.FieldInfo> opcodeToField = new();

		private void CompileOpcodeVerbose(ILProcessor il, OpcodeInformation opcode) {

		}

		private void CompileOpcodeInline(ILProcessor il, OpcodeInformation opcode) {
			throw new NotImplementedException();
		}

		private void CompileOpcodeUnsafe(ILProcessor il, OpcodeInformation opcode) {
			throw new NotImplementedException();
		}
	}
}
