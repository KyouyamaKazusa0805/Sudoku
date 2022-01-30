using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for unique loop or bi-value oddagon steps.
/// </summary>
public interface IUniqueLoopOrBivalueOddagonStepSearcher : IStepSearcher, ILoopLikeStepSearcher
{
	/// <summary>
	/// Searches for possible bi-value oddagon or unique loop patterns.
	/// </summary>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <param name="cell">The current cell calculated.</param>
	/// <param name="lastRegion">Indicates the last region type used.</param>
	/// <param name="exDigitsMask">The extra digits mask.</param>
	/// <param name="allowedExtraCellsCount">Indicates the number of cells can be with extra digits.</param>
	/// <param name="loopMap">Indicates the map of the loop.</param>
	/// <param name="tempLoop">Indicates the cells used of the loop, queued.</param>
	/// <param name="predicate">
	/// Indicates the condition that checks whether the procedured structure is a valid loop.
	/// </param>
	/// <param name="loops">The possible loops found.</param>
	protected static void SearchForPossibleLoopPatterns(
		in Grid grid, int d1, int d2, int cell, Region lastRegion, short exDigitsMask,
		int allowedExtraCellsCount, ref Cells loopMap, List<int> tempLoop,
		Func<bool> predicate, List<(Cells, IList<(Link, ColorIdentifier)>)> loops)
	{
		loopMap.AddAnyway(cell);
		tempLoop.Add(cell);

		foreach (var region in Regions)
		{
			if (region == lastRegion)
			{
				continue;
			}

			int regionIndex = cell.ToRegionIndex(region);
			var cellsMap = RegionMaps[regionIndex] & EmptyMap - cell;
			if (cellsMap.IsEmpty)
			{
				continue;
			}

			foreach (int nextCell in cellsMap)
			{
				if (tempLoop[0] == nextCell && tempLoop.Count >= 6 && predicate())
				{
					// The loop is closed. Now construct the result pair.
					loops.Add((loopMap, GetLinks(tempLoop)));
				}
				else if (!loopMap.Contains(nextCell) && grid[nextCell, d1] && grid[nextCell, d2])
				{
					// Here, unique loop can be found if and only if
					// two cells both contain 'd1' and 'd2'.
					// Incomplete ULs can't be found at present.
					short nextCellMask = grid.GetCandidates(nextCell);
					exDigitsMask |= nextCellMask;
					exDigitsMask &= (short)~((1 << d1) | (1 << d2));
					int digitsCount = PopCount((uint)nextCellMask);

					// We can continue if:
					// - The cell has exactly 2 digits of the loop.
					// - The cell has 1 extra digit, the same as all previous cells
					// with an extra digit (for type 2 only).
					// - The cell has extra digits and the maximum number of cells
					// with extra digits, 2, is not reached.
					if (digitsCount != 2 && !IsPow2(exDigitsMask) && allowedExtraCellsCount <= 0)
					{
						continue;
					}

					SearchForPossibleLoopPatterns(
						grid, d1, d2, nextCell, region, exDigitsMask,
						digitsCount > 2 ? allowedExtraCellsCount - 1 : allowedExtraCellsCount,
						ref loopMap, tempLoop, predicate, loops
					);
				}
			}
		}

		// Backtrack.
		loopMap.Remove(cell);
		tempLoop.RemoveAt(tempLoop.Count - 1);
	}
}