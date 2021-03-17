using Sudoku.DocComments;

namespace System.Extensions
{
	partial class BitOperationsEx
	{
		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this byte @this, int index)
		{
			for (int i = index + 1; i < 8; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this short @this, int index)
		{
			for (int i = index + 1; i < 16; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this int @this, int index)
		{
			for (int i = index + 1; i < 32; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this long @this, int index)
		{
			for (int i = index + 1; i < 64; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
