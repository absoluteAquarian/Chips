using Chips.Utility;
using System.Text;

namespace Chips.Compilation{
	internal abstract class CPDBFileInfo{
		private byte[] data;

		public ReadOnlySpan<byte> Data => new(data);

		public readonly CPDBClassification classification;

		public CPDBFileInfo(byte[] data, CPDBClassification classification){
			this.data = data;
			this.classification = classification;
		}

		protected string GetStringFromData(ref int readIndex){
			//Length is a 7-bit encoded int corresponding to the GetByteCount value for the original string
			int parentNameLength = data.Get7BitEncodedInt(readIndex, out int bytesRead);
			readIndex += bytesRead;

			string str = Encoding.UTF8.GetString(data, readIndex, parentNameLength);
			readIndex += parentNameLength;

			return str;
		}
	}

	internal class CPDBGlobalVariable : CPDBFileInfo{
		public readonly string variableName;

		public readonly int variableIndex;

		internal static int NextVariableIndex;

		public CPDBGlobalVariable(byte[] data) : base(data, CPDBClassification.GlobalVariable){
			variableIndex = NextVariableIndex++;

			variableName = Encoding.UTF8.GetString(data);
		}
	}

	internal class CPDBLocalVariable : CPDBFileInfo{
		public readonly string parentMethod;

		public readonly string variableName;

		public readonly int variableIndex;

		public CPDBLocalVariable(byte[] data) : base(data, CPDBClassification.LocalVariable){
			int index = 0;

			parentMethod = GetStringFromData(ref index);

			variableName = GetStringFromData(ref index);

			variableIndex = data.Get7BitEncodedInt(index, out _);
		}
	}

	internal class CPDBFunctionLabel : CPDBFileInfo{
		public readonly string parentMethod;

		public readonly int labelIndex;

		public readonly int opcodeIndex;

		public CPDBFunctionLabel(byte[] data) : base(data, CPDBClassification.Label){
			int index = 0;

			parentMethod = GetStringFromData(ref index);

			labelIndex = data.Get7BitEncodedInt(index, out int bytesRead);
			index += bytesRead;

			opcodeIndex = data.Get7BitEncodedInt(index, out _);
		}
	}
}
