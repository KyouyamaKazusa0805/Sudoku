namespace System.Numerics;

public partial class BitOperationsExtensions
{
	/// <summary>
	/// Find an index of the binary representation of a value after the specified index whose bit is set <see langword="true"/>.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="index">The index. The value will be automatically plus 1 in loop. Don't pass the value added 1.</param>
	/// <returns>The index.</returns>
	public static partial int GetNextSet(this byte @this, int index)
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

	/// <inheritdoc cref="GetNextSet(byte, int)"/>
	public static partial int GetNextSet(this short @this, int index)
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

	/// <inheritdoc cref="GetNextSet(byte, int)"/>
	public static partial int GetNextSet(this int @this, int index)
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

	/// <inheritdoc cref="GetNextSet(byte, int)"/>
	public static partial int GetNextSet(this long @this, int index)
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
