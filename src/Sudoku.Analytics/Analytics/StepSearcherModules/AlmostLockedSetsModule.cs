using Sudoku.Analytics.StepSearchers;
using Sudoku.Concepts;
using Sudoku.Runtime.CompilerServices;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents for the module that will be used for searching for almost locked sets.
/// </summary>
internal sealed class AlmostLockedSetsModule : IStepSearcherModule<AlmostLockedSetsModule>
{
	/// <inheritdoc/>
	static Type[] IStepSearcherModule<AlmostLockedSetsModule>.SupportedTypes
		=> [
			typeof(AlmostLockedSetsXzStepSearcher),
			typeof(AlmostLockedSetsXyWingStepSearcher),
			typeof(AlmostLockedSetsWWingStepSearcher),
			typeof(EmptyRectangleIntersectionPairStepSearcher)
		];


	/// <summary>
	/// Try to collect all possible ALSes in the specified grid.
	/// </summary>
	/// <param name="context">The grid to be used.</param>
	/// <returns>A list of ALSes.</returns>
	public static ReadOnlySpan<AlmostLockedSet> CollectAlmostLockedSets(scoped ref readonly AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (var cell in BivalueCells)
		{
			result.Add(new(grid.GetCandidates(cell), in CellsMap[cell], PeersMap[cell] & EmptyCells));
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

					var coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							in map,
							houseIndex < 9 && coveredLine is >= 9 and not InvalidTrailingZeroCountMethodFallback
								? ((HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return result.GetSpan();
	}

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
