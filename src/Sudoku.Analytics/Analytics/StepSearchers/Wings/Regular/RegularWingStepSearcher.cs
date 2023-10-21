using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Regular Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>XY-Wing</item>
/// <item>XYZ-Wing</item>
/// <item>WXYZ-Wing</item>
/// <item>VWXYZ-Wing</item>
/// <item>UVWXYZ-Wing</item>
/// <item>TUVWXYZ-Wing</item>
/// <item>STUVWXYZ-Wing</item>
/// <item>RSTUVWXYZ-Wing</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.XyWing, Technique.XyzWing, Technique.WxyzWing, Technique.VwxyzWing,
	Technique.UvwxyzWing, Technique.TuvwxyzWing, Technique.StuvwxyzWing, Technique.RstuvwxyzWing,
	Technique.IncompleteWxyzWing, Technique.IncompleteVwxyzWing, Technique.IncompleteUvwxyzWing,
	Technique.IncompleteTuvwxyzWing, Technique.IncompleteStuvwxyzWing, Technique.IncompleteRstuvwxyzWing)]
public sealed partial class RegularWingStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the maximum number of pivots to be searched for. The default value is 3.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.MaxSizeOfRegularWing)]
	public int MaxSearchingPivotsCount { get; set; } = 3;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Iterate on the size.
		// Note that the greatest size is determined by two factors: the size that you specified
		// and the number of bi-value cells in the grid.
		scoped ref readonly var grid = ref context.Grid;
		for (var size = 3; size <= Math.Min(MaxSearchingPivotsCount, BivalueCells.Count); size++)
		{
			// Iterate on each pivot cell.
			foreach (var pivot in EmptyCells)
			{
				var mask = grid.GetCandidates(pivot);
				var candsCount = PopCount((uint)mask);
				if (candsCount != size && candsCount != size - 1)
				{
					// Candidates are not enough.
					continue;
				}

				var map = PeersMap[pivot] & BivalueCells;
				if (map.Count < size - 1)
				{
					// Bi-value cells are not enough.
					continue;
				}

				// Iterate on each cell combination.
				foreach (ref readonly var cells in map.GetSubsets(size - 1).AsReadOnlySpan())
				{
					// Check duplicate.
					// If two cells contain same candidates, the wing can't be formed.
					var flag = false;
					for (var (i, length) = (0, cells.Count); i < length - 1; i++)
					{
						for (var j = i + 1; j < length; j++)
						{
							if (grid[cells[i]] == grid[cells[j]])
							{
								flag = true;
								goto CheckWhetherTwoCellsContainSameCandidateKind;
							}
						}
					}

				CheckWhetherTwoCellsContainSameCandidateKind:
					if (flag)
					{
						continue;
					}

					var union = mask;
					var inter = (Mask)(Grid.MaxCandidatesMask & mask);
					foreach (var cell in cells)
					{
						var m = grid.GetCandidates(cell);
						union |= m;
						inter &= m;
					}

					if (PopCount((uint)union) != size || inter != 0 && !IsPow2(inter))
					{
						continue;
					}

					// Get the Z digit (The digit to be removed).
					var isIncomplete = inter == 0;
					var interWithoutPivot = (Mask)(union & ~grid.GetCandidates(pivot));
					var maskToCheck = isIncomplete ? interWithoutPivot : inter;
					if (!IsPow2(maskToCheck))
					{
						continue;
					}

					// The pattern should be "az, bz, cz, dz, ... , abcd(z)".
					var zDigit = TrailingZeroCount(maskToCheck);
					var petals = cells;
					if ((petals + pivot & CandidatesMap[zDigit]).Count != (isIncomplete ? size - 1 : size))
					{
						continue;
					}

					// Check elimination map.
					var elimMap = petals.PeerIntersection;
					if (!isIncomplete)
					{
						elimMap &= PeersMap[pivot];
					}
					elimMap &= CandidatesMap[zDigit];
					if (!elimMap)
					{
						continue;
					}

					// Gather highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>(6);
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(digit == zDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}
					foreach (var digit in grid.GetCandidates(pivot))
					{
						candidateOffsets.Add(new(digit == zDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, pivot * 9 + digit));
					}

					var step = new RegularWingStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, zDigit)],
						[[.. candidateOffsets]],
						context.PredefinedOptions,
						pivot,
						PopCount((uint)mask),
						union,
						in petals
					);

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
