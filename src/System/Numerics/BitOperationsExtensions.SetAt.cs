namespace System.Numerics;

partial class BitOperationsExtensions
{
	/// <summary>
	/// Get an <see cref="Offset"/> value, indicating that the absolute position of all set bits with the specified set bit order.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="order">The number of the order of set bits.</param>
	/// <returns>The position.</returns>
	public static partial Offset SetAt(this byte @this, Count order)
	{
		for (Offset i = 0, count = -1; i < sizeof(byte) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0 && ++count == order)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="SetAt(byte, Count)"/>
	public static partial Offset SetAt(this short @this, Count order)
	{
		for (Offset i = 0, count = -1; i < sizeof(short) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0 && ++count == order)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="SetAt(byte, Count)"/>
	public static partial Offset SetAt(this int @this, Count order)
	{
		for (Offset i = 0, count = -1; i < sizeof(int) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0 && ++count == order)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="SetAt(byte, Count)"/>
	public static partial Offset SetAt(this long @this, Count order)
	{
		for (Offset i = 0, count = -1; i < sizeof(long) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0 && ++count == order)
			{
				return i;
			}
		}

		return -1;
	}
}
