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
}
