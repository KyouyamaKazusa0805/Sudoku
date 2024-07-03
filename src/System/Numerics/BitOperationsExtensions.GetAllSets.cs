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

		var (result, p) = (new int[PopCount((uint)@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount((uint)@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount((uint)@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount((ulong)@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
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

		var (result, p) = (new int[PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = TrailingZeroCount(@this);
			@this &= @this - 1;
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

		var (result, p) = (new int[(int)llong.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = (int)llong.TrailingZeroCount(@this);
			@this &= @this - 1;
		}
		return result;
	}

	/// <inheritdoc cref="GetAllSets(sbyte)"/>
	public static partial ReadOnlySpan<int> GetAllSets(this ullong @this)
	{
		if (@this == 0)
		{
			return [];
		}

		var (result, p) = (new int[(int)ullong.PopCount(@this)], 0);
		while (@this != 0)
		{
			result[p++] = (int)ullong.TrailingZeroCount(@this);
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
	public static partial ReadOnlySpan<int> GetAllSets<T>(this T @this) where T : IBinaryInteger<T>
	{
		if (@this == T.Zero)
		{
			return [];
		}

		var (result, p) = (new int[int.CreateChecked(T.PopCount(@this))], 0);
		while (@this != T.Zero)
		{
			result[p++] = int.CreateChecked(T.TrailingZeroCount(@this));
			@this &= @this - T.One;
		}
		return result;
	}
}
