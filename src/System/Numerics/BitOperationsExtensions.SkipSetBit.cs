namespace System.Numerics;

partial class BitOperationsExtensions
{
	/// <summary>
	/// Skip the specified number of set bits and iterate on the integer with other set bits.
	/// </summary>
	/// <param name="this">The integer to iterate.</param>
	/// <param name="setBitPosCount">Indicates how many set bits you want to skip to iterate.</param>
	/// <returns>The byte value that only contains the other set bits.</returns>
	/// <remarks>
	/// For example:
	/// <code><![CDATA[
	/// byte value = 0b00010111;
	/// foreach (int bitPos in value.SkipSetBit(2))
	/// {
	///     yield return bitPos + 1;
	/// }
	/// ]]></code>
	/// You will get 3 and 5, because all set bit positions are 0, 1, 2 and 4, and we have skipped
	/// two of them, so the result set bit positions to iterate on are only 2 and 4.
	/// </remarks>
	public static partial byte SkipSetBit(this byte @this, int setBitPosCount)
	{
		var result = @this;
		for (var (i, count) = (0, 0); i < 8; i++)
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

	/// <inheritdoc cref="SkipSetBit(byte, int)"/>
	public static partial short SkipSetBit(this short @this, int setBitPosCount)
	{
		var result = @this;
		for (var (i, count) = (0, 0); i < 16; i++)
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

	/// <inheritdoc cref="SkipSetBit(byte, int)"/>
	public static partial int SkipSetBit(this int @this, int setBitPosCount)
	{
		var result = @this;
		for (var (i, count) = (0, 0); i < 32; i++)
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

	/// <inheritdoc cref="SkipSetBit(byte, int)"/>
	public static partial long SkipSetBit(this long @this, int setBitPosCount)
	{
		var result = @this;
		for (var (i, count) = (0, 0); i < 64; i++)
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
