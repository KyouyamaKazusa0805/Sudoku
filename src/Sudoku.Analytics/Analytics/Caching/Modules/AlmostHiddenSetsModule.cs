namespace Sudoku.Analytics.Caching.Modules;

/// <summary>
/// Represents for the module that will be used for searching for almost hidden sets.
/// </summary>
internal static class AlmostHiddenSetsModule
{
	/// <summary>
	/// Try to collect all possible AHSes in the specified grid.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <returns>A list of AHSes.</returns>
	public static ReadOnlySpan<AlmostHiddenSetPattern> CollectAlmostHiddenSets(ref readonly Grid grid)
	{
		var result = new List<AlmostHiddenSetPattern>();
		var tempPosMask = (stackalloc Mask[9]);
		for (var house = 0; house < 27; house++)
		{
			var emptyCells = HousesMap[house] & EmptyCells;
			if (emptyCells.Count < 2)
			{
				// This house doesn't contain any valid AHSes.
				continue;
			}

			// Collect subview masks to determine whether AHSes of such digits can be formed.
			tempPosMask.Clear();
			var digits = grid[emptyCells].GetAllSets();
			foreach (var digit in digits)
			{
				tempPosMask[digit] = (HousesMap[house] & CandidatesMap[digit]) / house;
			}

			// Iterate on each combination of digit, to determine whether the digits in this combination of size (size)
			// is equal to (size - 1) cells.
			for (var size = 2; size <= emptyCells.Count - 1; size++)
			{
				foreach (var digitCombination in digits.GetSubsets(size))
				{
					var pos = (Mask)0;
					foreach (var digit in digitCombination)
					{
						pos |= tempPosMask[digit];
					}

					if (Mask.PopCount(pos) - 1 != size)
					{
						continue;
					}

					var cells = (from p in pos select HousesCells[house][p]).AsCellMap();
					var subsetDigitsMask = MaskOperations.Create(digitCombination);
					var allDigitsMask = grid[cells];
					var candidatesCanFormWeakLink = CandidateMap.Empty;
					foreach (var cell in cells)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~subsetDigitsMask))
						{
							candidatesCanFormWeakLink.Add(cell * 9 + digit);
						}
					}
					result.Add(new(in cells, house, allDigitsMask, subsetDigitsMask, in candidatesCanFormWeakLink));
				}
			}
		}
		return result.AsReadOnlySpan();
	}
}
