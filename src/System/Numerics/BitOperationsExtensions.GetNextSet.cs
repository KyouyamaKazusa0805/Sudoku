namespace System.Numerics;

partial class BitOperationsExtensions
{
	/// <summary>
	/// Find an index of the binary representation of a value after the specified index whose bit is set <see langword="true"/>.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="index">The index.</param>
	/// <returns>The index.</returns>
	public static partial Offset GetNextSet(this byte @this, Offset index)
	{
		for (var i = index + 1; i < 8; i++)
		{
			if ((@this >> i & 1) != 0)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="GetNextSet(byte, Offset)"/>
	public static partial Offset GetNextSet(this short @this, Offset index)
	{
		for (var i = index + 1; i < 16; i++)
		{
			if ((@this >> i & 1) != 0)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="GetNextSet(byte, Offset)"/>
	public static partial Offset GetNextSet(this int @this, Offset index)
	{
		for (var i = index + 1; i < 32; i++)
		{
			if ((@this >> i & 1) != 0)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="GetNextSet(byte, Offset)"/>
	public static partial Offset GetNextSet(this long @this, Offset index)
	{
		for (var i = index + 1; i < 64; i++)
		{
			if ((@this >> i & 1) != 0)
			{
				return i;
			}
		}

		return -1;
	}
}
