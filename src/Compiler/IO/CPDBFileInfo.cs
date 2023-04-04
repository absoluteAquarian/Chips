using Chips.Utility;
using System;

namespace Chips.IO {
	internal abstract class CPDBFileInfo {
		private readonly byte[] data;

		public ReadOnlySpan<byte> Data => new(data);

		public readonly CPDBClassification classification;

		public CPDBFileInfo(byte[] data, CPDBClassification classification) {
			this.data = data;
			this.classification = classification;
		}
	}

	internal class CPDBLocalVariable : CPDBFileInfo {
		public readonly string parentMethod;

		public readonly string variableName;

		public readonly int variableIndex;

		public CPDBLocalVariable(byte[] data) : base(data, CPDBClassification.LocalVariable) {
			int index = 0;

			parentMethod = data.GetStringFromData(ref index);

			variableName = data.GetStringFromData(ref index);

			variableIndex = data.Get7BitEncodedInt(index, out _);
		}
	}

	internal class CPDBFunctionLabel : CPDBFileInfo {
		public readonly string parentMethod;

		public readonly string labelName;

		public readonly int labelIndex;

		public readonly int opcodeIndex;

		public CPDBFunctionLabel(byte[] data) : base(data, CPDBClassification.Label) {
			int index = 0;

			parentMethod = data.GetStringFromData(ref index);

			labelName = data.GetStringFromData(ref index);

			labelIndex = data.Get7BitEncodedInt(index, out int bytesRead);
			index += bytesRead;

			opcodeIndex = data.Get7BitEncodedInt(index, out _);
		}
	}
}
