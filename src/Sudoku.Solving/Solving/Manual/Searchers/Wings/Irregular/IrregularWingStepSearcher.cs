using System.Numerics;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates an <b>irregular wing</b> technique searcher.
	/// </summary>
	public sealed class IrregularWingStepSearcher : StepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(7, DisplayingLevel.B);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(7, nameof(Technique.WWing))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		/// <remarks>
		/// In fact, <c>Hybrid-Wing</c>s, <c>Local-Wing</c>s, <c>Split-Wing</c>s and <c>M-Wing</c>s can
		/// be found in another searcher. In addition, these wings are not elementary and necessary techniques
		/// so we doesn't need to list them.
		/// </remarks>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			if (BivalueMap.Count < 2)
			{
				// The grid with possible W-Wing structure should
				// contain at least two empty cells (start and end cell).
				return;
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
						if (region == c1.ToRegion(RegionLabel.Block)
							|| region == c1.ToRegion(RegionLabel.Row)
							|| region == c1.ToRegion(RegionLabel.Column)
							|| region == c2.ToRegion(RegionLabel.Block)
							|| region == c2.ToRegion(RegionLabel.Row)
							|| region == c2.ToRegion(RegionLabel.Column))
						{
							// The region to search for conjugate pairs shouldn't
							// be the same as those two cells' regions.
							continue;
						}

						// Iterate on each digit to search for the conjugate pair.
						foreach (int digit in digits)
						{
							// Now search for conjugate pair.
							var conjugateRegion = CandMaps[digit] & RegionMaps[region];
							if (conjugateRegion.Count != 2)
							{
								// The current region doesn't contain the conjugate pair of this digit.
								continue;
							}

							// Check whether the cells are the same region as the head and the tail cell.
							int a = conjugateRegion[0], b = conjugateRegion[1];
							if (
								!(
									new Cells { c1, a }.InOneRegion && new Cells { c2, b }.InOneRegion
									|| new Cells { c1, b }.InOneRegion && new Cells { c2, a }.InOneRegion
								)
							)
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

							// Gather the eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int cell in elimMap)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, anotherDigit));
							}

							// Now W-Wing found. Store it into the accumulator.
							accumulator.Add(
								new WWingStepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Candidates = new DrawingInfo[]
											{
												new(0, c1 * 9 + anotherDigit),
												new(0, c2 * 9 + anotherDigit),
												new(1, c1 * 9 + digit),
												new(1, c2 * 9 + digit),
												new(1, a * 9 + digit),
												new(1, b * 9 + digit)
											},
											Regions = new DrawingInfo[] { new(0, region) }
										}
									},
									a,
									b,
									new(conjugateRegion, digit)
								)
							);
						}
					}
				}
			}
		}
	}
}
