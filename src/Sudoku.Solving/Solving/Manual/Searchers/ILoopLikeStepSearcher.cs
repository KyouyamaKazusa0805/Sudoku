using Sudoku.Presentation;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for loop-like steps.
/// </summary>
public interface ILoopLikeStepSearcher : IStepSearcher
{
	/// <summary>
	/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
	/// </summary>
	/// <param name="this">The list of cells.</param>
	/// <param name="offset">The offset. The default value is <c>4</c>.</param>
	/// <returns>All links.</returns>
	protected static IList<(Link, ColorIdentifier)> GetLinks(IReadOnlyList<int> @this, int offset = 4)
	{
		var result = new List<(Link, ColorIdentifier)>();

		for (int i = 0, length = @this.Count - 1; i < length; i++)
		{
			result.Add(
				(
					new(@this[i] * 9 + offset, @this[i + 1] * 9 + offset, LinkKind.Line),
					(ColorIdentifier)0
				)
			);
		}

		result.Add((new(@this[^1] * 9 + offset, @this[0] * 9 + offset, LinkKind.Line), (ColorIdentifier)0));

		return result;
	}
}