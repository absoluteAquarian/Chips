using Chips.Runtime;
using Chips.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Chips.IO {
	internal class CPDBFile {
		private readonly Dictionary<CPDBClassification, List<CPDBFileInfo>> information = new();

		public CPDBFile(string file) {
			if (Path.GetExtension(file) != ".cpdb")
				throw new IOException("File extension was not \".cpdb\"");

			using BinaryReader reader = new(File.OpenRead(file));

			Span<byte> header = stackalloc byte[7];
			reader.Read(header);

			if (header.DecodeSpan() != "CPDB\x1A\xEE\xFE")
				throw new IOException("Magic header was invalid");

			string version = reader.ReadString();
			if (version != Sandbox.Version)
				throw new IOException($"Chips PDB file \"{Path.GetFileName(file)}\" was built for Chips version v{version}, but the current version of Chips is v{Sandbox.Version}");

			long streamLength = reader.BaseStream.Length;

			while (reader.BaseStream.Position < streamLength) {
				CPDBClassification classification = (CPDBClassification)reader.ReadByte();

				if (!information.TryGetValue(classification, out var list))
					information[classification] = list = new();

				list.Add(ConstructInfo(classification, reader));
			}
		}

		private static CPDBFileInfo ConstructInfo(CPDBClassification classification, BinaryReader reader) {
			int dataLength = reader.Read7BitEncodedInt();

			byte[] data = reader.ReadBytes(dataLength);

			return classification switch {
				CPDBClassification.LocalVariable => new CPDBLocalVariable(data),
				CPDBClassification.Label => new CPDBFunctionLabel(data),
				_ => throw new ArgumentException("Unknown classification: " + classification)
			};
		}

		public bool TryFindLocalVariable(string method, string name, out CPDBLocalVariable? variable) {
			var list = information[CPDBClassification.LocalVariable];

			for (int i = 0; i < list.Count; i++) {
				var local = list[i] as CPDBLocalVariable;

				if (local?.parentMethod == method && local.variableName == name) {
					variable = local;
					return true;
				}
			}

			variable = null;
			return false;
		}

		public bool TryFindLocalVariableByIndex(string method, int index, out CPDBLocalVariable? variable) {
			var list = information[CPDBClassification.LocalVariable];

			for (int i = 0; i < list.Count; i++) {
				var local = list[i] as CPDBLocalVariable;

				if (local?.parentMethod == method && local.variableIndex == index) {
					variable = local;
					return true;
				}
			}

			variable = null;
			return false;
		}

		public bool TryFindFunctionLabel(string method, string name, out CPDBFunctionLabel? label) {
			var list = information[CPDBClassification.Label];

			for (int i = 0; i < list.Count; i++) {
				var funcLabel = list[i] as CPDBFunctionLabel;

				if (funcLabel?.parentMethod == method && funcLabel.labelName == name) {
					label = funcLabel;
					return true;
				}
			}

			label = null;
			return false;
		}

		public bool TryFindFunctionLabelByIndex(string method, int index, out CPDBFunctionLabel? label) {
			var list = information[CPDBClassification.Label];

			for (int i = 0; i < list.Count; i++) {
				var funcLabel = list[i] as CPDBFunctionLabel;

				if (funcLabel?.parentMethod == method && funcLabel.labelIndex == index) {
					label = funcLabel;
					return true;
				}
			}

			label = null;
			return false;
		}
	}
}
