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

		var (result, p) = (new int[sbyte.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = sbyte.TrailingZeroCount(@this);
			@this &= (sbyte)(@this - 1);
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

		var (result, p) = (new int[byte.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = byte.TrailingZeroCount(@this);
			@this &= (byte)(@this - 1);
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

		var (result, p) = (new int[short.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = short.TrailingZeroCount(@this);
			@this &= (short)(@this - 1);
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

		var (result, p) = (new int[ushort.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = ushort.TrailingZeroCount(@this);
			@this &= (ushort)(@this - 1);
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

		var (result, p) = (new int[int.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = int.TrailingZeroCount(@this);
			@this &= @this - 1;
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

		var (result, p) = (new int[uint.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = BitOperations.TrailingZeroCount(@this);
			@this &= @this - 1;
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

		var (result, p) = (new int[long.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = BitOperations.TrailingZeroCount(@this);
			@this &= @this - 1;
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

		var (result, p) = (new int[ulong.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = BitOperations.TrailingZeroCount(@this);
			@this &= @this - 1;
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this Int128 @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var (result, p) = (new int[(int)Int128.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = (int)Int128.TrailingZeroCount(@this);
			@this &= @this - 1;
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this UInt128 @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var (result, p) = (new int[(int)UInt128.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = (int)UInt128.TrailingZeroCount(@this);
			@this &= @this - 1;
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

		var (result, p) = (new int[(int)nint.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = (int)nint.TrailingZeroCount(@this);
			@this &= @this - 1;
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this nuint @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var (result, p) = (new int[(int)nuint.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = (int)nuint.TrailingZeroCount(@this);
			@this &= @this - 1;
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets<TInteger>(this TInteger @this) where TInteger : IBinaryInteger<TInteger>
	{
		if (@this == TInteger.Zero)
		{
			return [];
		}

		var (result, p) = (new int[int.CreateChecked(TInteger.PopCount(@this))], 0);
		while (@this != TInteger.Zero)
		{
			result[p++] = int.CreateChecked(TInteger.TrailingZeroCount(@this));
			@this &= @this - TInteger.One;
		}
		return result;
	}
}
