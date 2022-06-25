namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for guardian steps.
/// </summary>
public interface IGuardianStepSearcher : ISingleDigitPatternStepSearcher
{
	/// <summary>
	/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
	/// </summary>
	/// <param name="cells">The list of cells.</param>
	/// <param name="offset">The offset. The default value is 4.</param>
	/// <returns>All links.</returns>
	protected static sealed IEnumerable<LinkViewNode> GetLinks(IReadOnlyList<int> cells, int offset = 4)
	{
		var result = new List<LinkViewNode>();

		for (int i = 0, length = cells.Count - 1; i < length; i++)
		{
			result.Add(
				new(
					DisplayColorKind.Normal,
					new(offset, Cells.Empty + cells[i]),
					new(offset, Cells.Empty + cells[i + 1]),
					Inference.Default
				)
			);
		}

		result.Add(
			new(
				DisplayColorKind.Normal,
				new(offset, Cells.Empty + cells[^1]),
				new(offset, Cells.Empty + cells[0]),
				Inference.Default
			)
		);

		return result;
	}

	/// <summary>
	/// Create the guardian map.
	/// </summary>
	/// <param name="cell1">The first cell.</param>
	/// <param name="cell2">The second cell.</param>
	/// <param name="digit">The current digit.</param>
	/// <param name="guardians">
	/// The current guardian cells.
	/// This map may not contain cells that lies in the house
	/// that <paramref name="cell1"/> and <paramref name="cell2"/> both lies in.
	/// </param>
	/// <returns>All guardians.</returns>
	protected static Cells CreateGuardianMap(int cell1, int cell2, int digit, in Cells guardians)
	{
		var tempMap = Cells.Empty;
		foreach (int coveredHouse in (Cells.Empty + cell1 + cell2).CoveredHouses)
		{
			tempMap |= HouseMaps[coveredHouse];
		}

		tempMap &= CandidatesMap[digit];
		tempMap |= guardians;

		return tempMap - cell1 - cell2;
	}
}
