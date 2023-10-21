using System.Buffers;
using static System.Numerics.BitOperations;

namespace System.Collections;

/// <summary>
/// Provides extension methods on <see cref="BitArray"/>.
/// </summary>
/// <seealso cref="BitArray"/>
public static class BitArrayExtensions
{
	/// <summary>
	/// Get the cardinality of the specified <see cref="BitArray"/>.
	/// </summary>
	/// <param name="this">The array.</param>
	/// <returns>The total number of bits set <see langword="true"/>.</returns>
	public static Count GetCardinality(this BitArray @this)
	{
		var integers = ArrayPool<int>.Shared.Rent((@this.Length >> 5) + 1);
		try
		{
			@this.CopyTo(integers, 0);
			return integers.Sum(static integer => PopCount((uint)integer));
		}
		finally
		{
			ArrayPool<int>.Shared.Return(integers);
		}
	}
}
