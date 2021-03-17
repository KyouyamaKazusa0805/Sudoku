using Sudoku.DocComments;

namespace System.Extensions
{
	partial class BitOperationsEx
	{
		/// <inheritdoc cref="Integer.SkipSetBit(Integer, int)"/>
		public static byte SkipSetBit(this byte @this, int setBitPosCount)
		{
			byte result = @this;
			for (int i = 0, count = 0; i < 8; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					result &= (byte)~(1 << i);

					if (++count == setBitPosCount)
					{
						break;
					}
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.SkipSetBit(Integer, int)"/>
		public static short SkipSetBit(this short @this, int setBitPosCount)
		{
			short result = @this;
			for (int i = 0, count = 0; i < 16; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					result &= (short)~(1 << i);

					if (++count == setBitPosCount)
					{
						break;
					}
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.SkipSetBit(Integer, int)"/>
		public static int SkipSetBit(this int @this, int setBitPosCount)
		{
			int result = @this;
			for (int i = 0, count = 0; i < 32; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					result &= ~(1 << i);

					if (++count == setBitPosCount)
					{
						break;
					}
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.SkipSetBit(Integer, int)"/>
		public static long SkipSetBit(this long @this, int setBitPosCount)
		{
			long result = @this;
			for (int i = 0, count = 0; i < 64; i++)
			{
				if ((@this >> i & 1) != 0)
				{
					result &= ~(1 << i);

					if (++count == setBitPosCount)
					{
						break;
					}
				}
			}

			return result;
		}
	}
}
