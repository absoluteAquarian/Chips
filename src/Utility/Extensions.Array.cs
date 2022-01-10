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
	}
}
