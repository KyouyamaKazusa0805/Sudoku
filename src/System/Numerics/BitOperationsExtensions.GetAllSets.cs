namespace System.Numerics;

public partial class BitOperationsExtensions
{
	/// <summary>
	/// Find all offsets of set bits of the binary representation of a specified value.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <returns>All offsets.</returns>
	public static partial ReadOnlySpan<int> GetAllSets(this sbyte @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount((uint)@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(sbyte) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this byte @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount(@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(byte) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this short @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount((uint)@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(short) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this ushort @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount(@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(ushort) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this int @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount((uint)@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(int) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this uint @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount(@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(uint) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this long @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount((ulong)@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(long) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this ulong @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var length = PopCount(@this);
		var result = new int[length];
		for (byte i = 0, p = 0; i < sizeof(ulong) << 3; i++, @this >>= 1)
		{
			if ((@this & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this llong @this)
	{
		if (@this == 0)
		{
			return [];
		}

		unsafe
		{
			var (upper, lower) = ((ulong)(@this >>> 64), (ulong)(@this & ulong.MaxValue));
			var length = PopCount(upper) + PopCount(lower);
			var result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(llong) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}
			return result;
		}
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this ullong @this)
	{
		if (@this == 0)
		{
			return [];
		}

		unsafe
		{
			var (upper, lower) = ((ulong)(@this >>> 64), (ulong)(@this & ulong.MaxValue));
			var length = PopCount(upper) + PopCount(lower);
			var result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(ullong) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}
			return result;
		}
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this nint @this)
	{
		if (@this == 0)
		{
			return [];
		}

		unsafe
		{
			var length = PopCount((nuint)@this);
			var result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(nint) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}
			return result;
		}
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this nuint @this)
	{
		if (@this == 0)
		{
			return [];
		}

		unsafe
		{
			var length = PopCount(@this);
			var result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(nuint) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}
			return result;
		}
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets<T>(this T @this) where T : IBinaryInteger<T>
	{
		if (@this == T.Zero)
		{
			return [];
		}

		unsafe
		{
			var result = new List<int>();
			for (byte i = 0, p = 0; i < sizeof(T) << 3; i++, @this >>= 1)
			{
				if ((@this & T.One) != T.Zero)
				{
					result[p++] = i;
				}
			}
			return result.AsReadOnlySpan();
		}
	}
}
