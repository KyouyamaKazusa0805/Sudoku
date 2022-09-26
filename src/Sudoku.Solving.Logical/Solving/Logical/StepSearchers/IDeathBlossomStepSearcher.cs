namespace Sudoku.Solving.Logical.StepSearchers;

using GatheredData = Dictionary</*Cell*/ int, Dictionary</*Digit*/ int, List<AlmostLockedSet>>>;

/// <summary>
/// Provides with a <b>Death Blossom</b> step searcher.
/// <!--
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Death Blossom Cell Type</item>
/// <item>Death Blossom Region Type (House Type)</item>
/// </list>
/// -->
/// </summary>
public interface IDeathBlossomStepSearcher : IAlmostLockedSetsStepSearcher
{
	/// <summary>
	/// <para>Gathers almost locked sets and groups them by cell related.</para>
	/// <para>
	/// For example, <c>r1c12(123)</c> is an almost locked set, and cells <c>{ r1c3, r23c123 }</c>
	/// can be related to that ALS.
	/// </para>
	/// </summary>
	/// <param name="grid">The grid used.</param>
	/// <returns>The dictionary of grouped result.</returns>
	protected internal static sealed GatheredData GatherGroupedByCell(scoped in Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new GatheredData();
		foreach (var cell in BivalueCells)
		{
			var als = new AlmostLockedSet(grid.GetCandidates(cell), CellsMap[cell], PeersMap[cell] & EmptyCells);
			foreach (var peerCell in PeersMap[cell])
			{
				append(als, peerCell, result);
			}
		}

		// Get all non-bi-value-cell ALSes.
		for (var houseIndex = 0; houseIndex < 27; houseIndex++)
		{
			if ((HousesMap[houseIndex] & EmptyCells) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (var size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && houseIndex >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					short digitsMask = 0;
					foreach (var cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					var coveredLine = map.CoveredLine;

					var als = new AlmostLockedSet(
						digitsMask,
						map,
						houseIndex < 9 && coveredLine is >= 9 and not InvalidValidOfTrailingZeroCountMethodFallback
							? ((HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells) - map
							: tempMap - map
					);
					foreach (var digit in digitsMask)
					{
						foreach (var peerCell in map % CandidatesMap[digit])
						{
							append(als, peerCell, result);
						}
					}
				}
			}
		}

		return result;


		static void append(AlmostLockedSet structure, int cell, GatheredData gatheredData)
		{
			foreach (var digit in structure.DigitsMask)
			{
				if (gatheredData.TryGetValue(cell, out var structuresGroupedByCell))
				{
					if (structuresGroupedByCell.TryGetValue(digit, out var structuresGroupedByCandidate))
					{
						structuresGroupedByCandidate.Add(structure);
					}
					else
					{
						structuresGroupedByCell.Add(digit, new() { structure });
					}
				}
				else
				{
					gatheredData.Add(cell, new() { { digit, new() { structure } } });
				}
			}
		}
	}
}
