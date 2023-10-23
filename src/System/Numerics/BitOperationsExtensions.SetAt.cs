namespace System.Numerics;

partial class BitOperationsExtensions
{
	/// <summary>
	/// Get an <see cref="int"/> value, indicating that the absolute position of all set bits with the specified set bit order.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="order">The number of the order of set bits.</param>
	/// <returns>The position.</returns>
	public static partial int SetAt(this byte @this, int order)
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

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this short @this, int order)
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

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this int @this, int order)
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

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this long @this, int order)
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
