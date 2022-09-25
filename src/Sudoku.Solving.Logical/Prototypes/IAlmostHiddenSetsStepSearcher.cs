namespace Sudoku.Solving.Logical.Prototypes;

/// <summary>
/// Provides with an <b>Almost Hidden Sets</b> step searcher.
/// </summary>
public interface IAlmostHiddenSetsStepSearcher : IStepSearcher
{
	/// <inheritdoc cref="AlmostHiddenSet.Gather(in Grid)"/>
	/// <remarks>
	/// Different with the original method <see cref="AlmostHiddenSet.Gather(in Grid)"/>,
	/// this method will only uses the buffer to determine the info, which is unsafe
	/// when calling the method without having initialized those maps in the buffer type,
	/// <see cref="FastProperties"/>.
	/// </remarks>
	/// <seealso cref="AlmostHiddenSet"/>
	/// <seealso cref="AlmostHiddenSet.Gather(in Grid)"/>
	/// <seealso cref="FastProperties"/>
	protected internal static sealed AlmostHiddenSet[] Gather(scoped in Grid grid)
	{
		var result = new List<AlmostHiddenSet>();

		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & EmptyCells) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			var digitsMask = grid.GetDigitsUnion(tempMap);
			for (var size = 2; size < tempMap.Count - 1; size++)
			{
				foreach (var digitCombination in digitsMask.GetAllSets().GetSubsets(size))
				{
					var cells = CellMap.Empty;
					foreach (var digit in digitCombination)
					{
						cells |= CandidatesMap[digit] & HousesMap[house];
					}
					if (cells.Count - 1 != size)
					{
						continue;
					}

					short finalDigitsMask = 0;
					foreach (var digit in digitCombination)
					{
						finalDigitsMask |= (short)(1 << digit);
					}

					var allDigitsMask = grid.GetDigitsUnion(cells);
					var finalMaps = new CellMap?[9];
					for (var digit = 0; digit < 9; digit++)
					{
						if ((finalDigitsMask >> digit & 1) != 0 || (allDigitsMask >> digit & 1) != 0)
						{
							finalMaps[digit] = CandidatesMap[digit] & cells;
						}
					}

					result.Add(new(finalDigitsMask, allDigitsMask, cells, finalMaps));
				}
			}
		}

		return result.ToArray();
	}
}
