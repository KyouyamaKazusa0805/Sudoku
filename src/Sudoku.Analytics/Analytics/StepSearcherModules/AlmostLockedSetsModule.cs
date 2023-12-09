using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents for the module that will be used for searching for almost locked sets.
/// </summary>
internal static class AlmostLockedSetsModule
{
	/// <summary>
	/// Try to collect all possible ALSes in the specified grid.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <returns>A list of ALSes.</returns>
	public static ReadOnlySpan<AlmostLockedSet> CollectAlmostLockedSets(scoped ref readonly Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (var cell in BivalueCells)
		{
			var eliminationMap = new CellMap[9];
			foreach (var digit in grid.GetCandidates(cell))
			{
				eliminationMap[digit] = PeersMap[cell] & CandidatesMap[digit];
			}

			result.Add(new(grid.GetCandidates(cell), in CellsMap[cell], PeersMap[cell] & EmptyCells, eliminationMap));
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
				foreach (ref readonly var map in tempMap.GetSubsets(size))
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && houseIndex >= 9)
					{
						// All ALS cells lying on a box-row or a box-column will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					var digitsMask = (Mask)0;
					foreach (var cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					var eliminationMap = new CellMap[9];
					foreach (var digit in digitsMask)
					{
						eliminationMap[digit] = map % CandidatesMap[digit];
					}

					var coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							in map,
							houseIndex < 9 && coveredLine is >= 9 and not InvalidTrailingZeroCountMethodFallback
								? ((HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells) - map
								: tempMap - map,
							eliminationMap
						)
					);
				}
			}
		}

		return CollectionsMarshal.AsSpan(result);
	}

	/// <summary>
	/// Try to fetch the ALS color.
	/// </summary>
	/// <param name="index">The index of the target ALS.</param>
	/// <returns>The color identifier.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ColorIdentifier GetColor(int index)
		=> (index % 5) switch
		{
			0 => WellKnownColorIdentifier.AlmostLockedSet1,
			1 => WellKnownColorIdentifier.AlmostLockedSet2,
			2 => WellKnownColorIdentifier.AlmostLockedSet3,
			3 => WellKnownColorIdentifier.AlmostLockedSet4,
			4 => WellKnownColorIdentifier.AlmostLockedSet5
		};

	/// <summary>
	/// Collect possible conjugate pairs grouped by digit.
	/// </summary>
	/// <returns>The conjugate pairs found, grouped by digit.</returns>
	internal static List<Conjugate>?[] CollectConjugatePairs()
	{
		var conjugatePairs = new List<Conjugate>?[9];
		for (var digit = 0; digit < 9; digit++)
		{
			for (var houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				if ((HousesMap[houseIndex] & CandidatesMap[digit]) is { Count: 2 } temp)
				{
					(conjugatePairs[digit] ??= []).Add(new(in temp, digit));
				}
			}
		}

		return conjugatePairs;
	}
}
