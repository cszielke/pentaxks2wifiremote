using System;

namespace PtxK_S2
{

	/// <summary>
	/// Some array utilities
	/// </summary>
	internal class ByteArrayUtils
	{
		// Check if the array contains needle on specified position
		public static bool Compare(byte[] array, byte[] needle, int startIndex)
		{
			int	needleLen = needle.Length;
			// compare
			for (int i = 0, p = startIndex; i < needleLen; i++, p++)
			{
				if (array[p] != needle[i])
				{
					return false;
				}
			}
			return true;
		}

		// Find subarray in array
		public static int Find(byte[] array, byte[] needle, int startIndex, int count)
		{
			int	needleLen = needle.Length;
			int	index;

			while (count >= needleLen)
			{
				index = Array.IndexOf(array, needle[0], startIndex, count - needleLen + 1);

				if (index == -1)
					return -1;

				int i, p;
				// check for needle
				for (i = 0, p = index; i < needleLen; i++, p++)
				{
					if (array[p] != needle[i])
					{
						break;
					}
				}

				if (i == needleLen)
				{
					// found needle
					return index;
				}

				count -= (index - startIndex + 1);
				startIndex = index + 1;
			}
			return -1;
		}
	}
}
