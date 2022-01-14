using System.Text;

namespace Chips.Utility{
	internal static partial class Extensions{
		public static int Get7BitEncodedInt(this byte[] array, int startIndex, out int bytesRead){
			int ret = 0;
			bytesRead = 0;

			byte read;

			do{
				if(bytesRead >= 5)
					throw new ArgumentException("Invalid 7-bit encoded Int32 format");

				read = array[startIndex + bytesRead];

				ret |= read & ~0x80;
				ret <<= 7;

				bytesRead++;
			}while((read & 0x80) != 0);

			return ret;
		}

		public static T[] GetSlice<T>(this T[] array, int start, int count) where T : unmanaged
			=> array.AsSpan()[start..(start + count)].ToArray();

		public static string GetStringFromData(this byte[] data, ref int readIndex){
			//Length is a 7-bit encoded int corresponding to the GetByteCount value for the original string
			int length = data.Get7BitEncodedInt(readIndex, out int bytesRead);
			readIndex += bytesRead;

			string str = Encoding.UTF8.GetString(data, readIndex, length);
			readIndex += length;

			return str;
		}
	}
}
