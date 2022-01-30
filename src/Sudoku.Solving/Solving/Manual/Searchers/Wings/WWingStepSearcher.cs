using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Steps.Wings;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers.Wings;

/// <summary>
/// Provides with a <b>W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>W-Wing (George Woods' Wing)</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class WWingStepSearcher : IIregularWingStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(7, DisplayingLevel.B);


	/// <inheritdoc/>
	/// <remarks>
	/// In fact, <c>Hybrid-Wing</c>s, <c>Local-Wing</c>s, <c>Split-Wing</c>s and <c>M-Wing</c>s can
	/// be found in another searcher. In addition, these wings are not elementary and necessary techniques
	/// so we doesn't need to list them.
	/// </remarks>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// The grid with possible W-Wing structure should contain
		// at least two empty cells (start and end cell).
		if (BivalueMap.Count < 2)
		{
			return null;
		}

		// Iterate on each cells.
		for (int c1 = 0; c1 < 72; c1++)
		{
			if (!BivalueMap.Contains(c1))
			{
				// The cell isn't a bi-value cell.
				continue;
			}

			// Iterate on each cells which are not peers in 'c1'.
			var digits = grid.GetCandidates(c1).GetAllSets();
			foreach (int c2 in BivalueMap - new Cells(c1))
			{
				if (c2 < c1)
				{
					// To avoid duplicate structures found.
					continue;
				}

				if (grid.GetCandidates(c1) != grid.GetCandidates(c2))
				{
					// Two cells may contain different kinds of digits.
					continue;
				}

				var intersection = PeerMaps[c1] & PeerMaps[c2];
				if ((EmptyMap & intersection).IsEmpty)
				{
					// The structure doesn't contain any possible eliminations.
					continue;
				}

				// Iterate on each region.
				for (int region = 0; region < 27; region++)
				{
					if (region == c1.ToRegionIndex(Region.Block)
						|| region == c1.ToRegionIndex(Region.Row)
						|| region == c1.ToRegionIndex(Region.Column)
						|| region == c2.ToRegionIndex(Region.Block)
						|| region == c2.ToRegionIndex(Region.Row)
						|| region == c2.ToRegionIndex(Region.Column))
					{
						// The region to search for conjugate pairs shouldn't
						// be the same as those two cells' regions.
						continue;
					}

					// Iterate on each digit to search for the conjugate pair.
					foreach (int digit in digits)
					{
						// Now search for conjugate pair.
						if ((CandMaps[digit] & RegionMaps[region]) is not [var a, var b] conjugate)
						{
							// The current region doesn't contain the conjugate pair of this digit.
							continue;
						}

						// Check whether the cells are the same region as the head and the tail cell.
						if (!new Cells { c1, a }.InOneRegion || !new Cells { c2, b }.InOneRegion
							&& !new Cells { c1, b }.InOneRegion || !new Cells { c2, a }.InOneRegion)
						{
							continue;
						}

						// Check for eliminations.
						int anotherDigit = TrailingZeroCount(grid.GetCandidates(c1) & ~(1 << digit));
						var elimMap = CandMaps[anotherDigit] & new Cells { c1, c2 }.PeerIntersection;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						// Now W-Wing found. Store it into the accumulator.
						var step = new WWingStep(
							elimMap.ToImmutableConclusions(anotherDigit),
							ImmutableArray.Create(new PresentationData
							{
								Candidates = new[]
								{
									(c1 * 9 + anotherDigit, (ColorIdentifier)0),
									(c2 * 9 + anotherDigit, (ColorIdentifier)0),
									(c1 * 9 + digit, (ColorIdentifier)1),
									(c2 * 9 + digit, (ColorIdentifier)1),
									(a * 9 + digit, (ColorIdentifier)1),
									(b * 9 + digit, (ColorIdentifier)1)
								},
								Regions = new[] { (region, (ColorIdentifier)0) }
							}),
							a,
							b,
							new(conjugate, digit)
						);

						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}
			}
		}

		return null;
	}
}
