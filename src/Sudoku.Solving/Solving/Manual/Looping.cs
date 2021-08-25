namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with a serial of methods that checks for loops.
/// </summary>
internal static class Looping
{
	/// <summary>
	/// Checks whether the specified loop of cells is a valid loop.
	/// </summary>
	/// <param name="loopCells">The loop cells.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static unsafe bool IsValidLoop(IList<int> loopCells)
	{
		int visitedOddRegions = 0, visitedEvenRegions = 0;
		bool isOdd;
		foreach (int cell in loopCells)
		{
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = cell.ToRegion(label);
				if (*&isOdd)
				{
					if ((visitedOddRegions >> region & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedOddRegions |= 1 << region;
					}
				}
				else
				{
					if ((visitedEvenRegions >> region & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedEvenRegions |= 1 << region;
					}
				}
			}

			*&isOdd = !*&isOdd;
		}

		return visitedEvenRegions == visitedOddRegions;
	}

	/// <summary>
	/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
	/// </summary>
	/// <param name="this">The list of cells.</param>
	/// <param name="offset">The offset. The default value is <c>4</c>.</param>
	/// <returns>All links.</returns>
	public static IList<(Link, ColorIdentifier)> GetLinks(this IReadOnlyList<int> @this, int offset = 4)
	{
		var result = new List<(Link, ColorIdentifier)>();

		for (int i = 0, length = @this.Count - 1; i < length; i++)
		{
			result.Add((new(@this[i] * 9 + offset, @this[i + 1] * 9 + offset, LinkType.Line), (ColorIdentifier)0));
		}

		result.Add((new(@this[^1] * 9 + offset, @this[0] * 9 + offset, LinkType.Line), (ColorIdentifier)0));

		return result;
	}
}