using Sudoku.DocComments;

namespace System.Extensions
{
	partial class BitOperationsEx
	{
		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this byte @this, int order)
		{
			for (int i = 0, count = -1; i < sizeof(byte) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this short @this, int order)
		{
			for (int i = 0, count = -1; i < sizeof(short) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this int @this, int order)
		{
			for (int i = 0, count = -1; i < sizeof(int) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this long @this, int order)
		{
			for (int i = 0, count = -1; i < sizeof(long) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
