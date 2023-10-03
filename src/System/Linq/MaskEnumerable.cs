using System.Numerics;
using static System.Numerics.BitOperations;

namespace System.Linq;

/// <summary>
/// Represents a set of operation that iterates on each bit in a <see cref="Mask"/> instance like LINQ.
/// </summary>
/// <seealso cref="Mask"/>
public static class MaskEnumerable
{
	/// <summary>
	/// Projects each bit from a specified mask, converting it into a (an) <typeparamref name="T"/> instance,
	/// with specified method to be called.
	/// </summary>
	/// <typeparam name="T">The target type of values for each bit converted.</typeparam>
	/// <param name="this">A mask instance.</param>
	/// <param name="selector">The selector method to be converted.</param>
	/// <returns>A list of converted result, encapsulated by a <see cref="ReadOnlySpan{T}"/> type.</returns>
	public static ReadOnlySpan<T> Select<T>(this Mask @this, Func<int, T> selector)
	{
		var (result, i) = (new T[PopCount((uint)@this)], 0);
		foreach (var bit in @this)
		{
			result[i++] = selector(bit);
		}

		return result;
	}

	/// <summary>
	/// Filters bits via the specified condition.
	/// </summary>
	/// <param name="this">The mask type of bits.</param>
	/// <param name="predicate">The condition that filters bits, removing bits not satisfying the condition.</param>
	/// <returns>A new <see cref="Mask"/> result.</returns>
	public static Mask Where(this Mask @this, Func<int, bool> predicate)
	{
		var result = (Mask)0;
		foreach (var bitPos in @this)
		{
			if (predicate(bitPos))
			{
				result |= (Mask)(1 << bitPos);
			}
		}
		return result;
	}
}
