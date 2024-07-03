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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial int SetAt(this long @this, int order) => SetAt((ulong)@this, order);

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt(this ulong @this, int order)
	{
		var (mask, size, @base) = (0x0000FFFFu, 16u, 0u);
		if (order++ >= PopCount(@this))
		{
			return -1;
		}

		while (size > 0)
		{
			if (order > PopCount(@this & mask))
			{
				@base += size;
				size >>= 1;
				mask |= mask << (int)size;
			}
			else
			{
				size >>= 1;
				mask >>= (int)size;
			}
		}
		return @base == 64 ? -1 : (int)@base;
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

	/// <inheritdoc cref="SetAt(byte, int)"/>
	public static partial int SetAt<TNumber>(this TNumber @this, int order)
#if NUMERIC_GENERIC_TYPE
		where TNumber : IBitwiseOperators<TNumber, TNumber, TNumber>, INumber<TNumber>, IShiftOperators<TNumber, int, TNumber>
#else
		where TNumber :
			IAdditiveIdentity<TNumber, TNumber>,
			IBitwiseOperators<TNumber, TNumber, TNumber>,
			IEqualityOperators<TNumber, TNumber, bool>,
			IMultiplicativeIdentity<TNumber, TNumber>,
			IShiftOperators<TNumber, int, TNumber>
#endif
	{
		unsafe
		{
			for (int i = 0, count = -1; i < sizeof(TNumber) << 3; i++, @this >>= 1)
			{
				if ((@this & TNumber.MultiplicativeIdentity) != TNumber.AdditiveIdentity && ++count == order)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
