using System;

namespace Chips.Runtime.Utility {
	internal static class Sorting {
		/// <summary>
		/// Performs the Tim Sort algorithm on the <paramref name="array"/>.  The input <paramref name="array"/> is modified by this method.
		/// </summary>
		/// <typeparam name="T">The generic type of the array.  Must inherit from <seealso cref="IComparable{T}"/></typeparam>
		/// <param name="array">The array to be sorted.  The array's contents are affected by calling this method.</param>
		public static void TimSort<T>(T[] array) where T : IComparable<T> {
			//The following code was ported from a C++ project
			//32 is used as a compromize between fast Insertion Sorts and fewer merges
			//A power of two is used for the most efficiency, since the resulting arrays will also be powers of two (unless the array length isn't, then the last one will just be the remainder.)
			//Insertion Sort is very efficient with small arrays, but not with large arrays.  A value of 64 or higher results in slower initial sorting, but faster merging
			//A value of 16 or lower results in faster initial sorting, but slower merging since more merging has to be done
			const int SUBARRAY_SIZE = 32;

			if (array is null)
				return;

			if (array.Length < 2)
				return;

			//Sort the subarrays using insertion sort
			for (int i = 0; i < array.Length; i += SUBARRAY_SIZE)
				InsertionSort(array, i, Math.Min(i + SUBARRAY_SIZE - 1, array.Length - 1));

			if (array.Length <= SUBARRAY_SIZE)
				return;

			//Merge the subarrays
			for (int size = SUBARRAY_SIZE; size < array.Length; size *= 2) {
				for (int left = 0; left < array.Length; left += 2 * size) {
					int mid = left + size - 1;
					int right = Math.Min(left + 2 * size - 1, array.Length - 1);

					//Sanity check for if the left subarray is the final subarray
					//'mid' ends up being greater than 'count' if this were the case, which causes problems in
					//  Merge() which sets the length of the right subarray to 'right - mid'
					if (mid >= array.Length)
						continue;

					Merge(array, left, mid, right);
				}
			}
		}

		//Helper methods for TimSort
		private static void InsertionSort<T>(T[] array, int start, int end) where T : IComparable<T> {
			for (int i = start + 1; i <= end; i++) {
				T index = array[i];
				int j = i - 1;
				while (j >= start && array[j]?.CompareTo(index) > 0) {
					array[j + 1] = array[j];
					j--;
				}
				array[j + 1] = index;
			}
		}

		private static void Merge<T>(T[] array, int left, int mid, int right) where T : IComparable<T> {
			//Original array is broken into two parts: left and right subarray
			T[] leftArr = new T[mid - left + 1];
			T[] rightArr = new T[right - mid];

			//Fill in the subarrays
			for (int index = 0; index < leftArr.Length; index++)
				leftArr[index] = array[left + index];
			for (int index = 0; index < rightArr.Length; index++)
				rightArr[index] = array[mid + 1 + index];

			//Merge the subarrays after comparing
			int i = 0, j = 0, k = left;
			while (i < leftArr.Length && j < rightArr.Length) {
				if ((leftArr[i]?.CompareTo(rightArr[j]) ?? 0) <= 0)
					array[k++] = leftArr[i++];
				else
					array[k++] = rightArr[j++];
			}

			//Copy any remaining elements left, if any
			while (i < leftArr.Length)
				array[k++] = leftArr[i++];
			while (j < rightArr.Length)
				array[k++] = rightArr[j++];
		}
	}
}
