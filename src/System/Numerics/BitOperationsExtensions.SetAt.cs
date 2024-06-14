namespace System.Numerics;

public partial class BitOperationsExtensions
{
	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this sbyte @this, int order)
	{
		for (int i = 0, count = -1; i < sizeof(sbyte) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0 && ++count == order)
			{
				return i;
			}
		}
		return -1;
	}

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
	public static partial int SetAt(this ushort @this, int order)
	{
		for (int i = 0, count = -1; i < sizeof(ushort) << 3; i++, @this >>= 1)
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
	public static partial int SetAt(this uint @this, int order)
	{
		for (int i = 0, count = -1; i < sizeof(uint) << 3; i++, @this >>= 1)
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

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this ulong @this, int order)
	{
		for (int i = 0, count = -1; i < sizeof(ulong) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0 && ++count == order)
			{
				return i;
			}
		}
		return -1;
	}

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this llong @this, int order)
	{
		unsafe
		{
			for (int i = 0, count = -1; i < sizeof(llong) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}
			return -1;
		}
	}

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this ullong @this, int order)
	{
		unsafe
		{
			for (int i = 0, count = -1; i < sizeof(ullong) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}
			return -1;
		}
	}

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this nint @this, int order)
	{
		unsafe
		{
			for (int i = 0, count = -1; i < sizeof(nint) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}
			return -1;
		}
	}

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this nuint @this, int order)
	{
		unsafe
		{
			for (int i = 0, count = -1; i < sizeof(nuint) << 3; i++, @this >>= 1)
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
