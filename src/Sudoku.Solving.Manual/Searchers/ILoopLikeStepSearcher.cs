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
	protected static IEnumerable<LinkViewNode> GetLinks(IReadOnlyList<int> @this, int offset = 4)
	{
		var result = new List<LinkViewNode>();

		for (int i = 0, length = @this.Count - 1; i < length; i++)
		{
			result.Add(
				new(
					0,
					new(offset, Cells.Empty + @this[i]),
					new(offset, Cells.Empty + @this[i + 1]),
					Inference.Default
				)
			);
		}

		result.Add(
			new(
				0,
				new(offset, Cells.Empty + @this[^1]),
				new(offset, Cells.Empty + @this[0]),
				Inference.Default
			)
		);

		return result;
	}
}
