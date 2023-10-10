using System.Numerics;
using Sudoku.Concepts;
using static System.Numerics.BitOperations;

namespace Sudoku.Linq;

/// <summary>
/// Represents a list of methods for iteration on a <see cref="Mask"/>.
/// </summary>
/// <seealso cref="Mask"/>
public static class MaskEnumerable
{
	/// <summary>
	/// Projects each <see cref="Digit"/> of a <see cref="Mask"/> to a <see cref="CellMap"/>, flattens the resulting sequence into one sequence,
	/// and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <param name="source">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input <see cref="Mask"/>.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate <see cref="CellMap"/>.</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> whose elements are the result of invoking the one-to-many transform function
	/// <paramref name="collectionSelector"/> on each element of <paramref name="source"/> and then mapping each of those sequences
	/// and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<TResult> SelectMany<TResult>(
		this Mask source,
		Func<Digit, CellMap> collectionSelector,
		Func<Digit, Cell, TResult> resultSelector
	)
	{
		var result = new List<TResult>(PopCount((uint)source) << 1);
		foreach (var digit in source)
		{
			foreach (var cell in collectionSelector(digit))
			{
				result.Add(resultSelector(digit, cell));
			}
		}

		return result.ToArray();
	}
}
