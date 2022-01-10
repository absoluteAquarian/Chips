using System.Text;

namespace Chips.Utility{
	internal static partial class Extensions{
		public static void WriteCPDBString(this BinaryWriter writer, string value){
			int count = Encoding.UTF8.GetByteCount(value);
			writer.Write7BitEncodedInt(count);

			writer.Write(Encoding.UTF8.GetBytes(value));
		}
	}
}
