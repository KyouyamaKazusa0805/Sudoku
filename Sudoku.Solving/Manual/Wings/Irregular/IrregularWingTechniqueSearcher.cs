using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates an <b>irregular wing</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.WWing))]
	[SearcherProperty(44)]
	public sealed class IrregularWingTechniqueSearcher : TechniqueSearcher
	{
		/// <inheritdoc/>
		/// <remarks>
		/// In fact, <c>Hybrid-Wing</c>s, <c>Local-Wing</c>s, <c>Split-Wing</c>s and <c>M-Wing</c>s can
		/// be found in another searcher. In addition, these wings are not elementary and necessary techniques
		/// so we does not need to list them.
		/// </remarks>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (BivalueMap.Count < 2)
			{
				return;
			}

			// Iterate on each cells.
			for (int c1 = 0; c1 < 72; c1++)
			{
				if (!BivalueMap[c1] || !EmptyMap[c1])
				{
					continue;
				}

				// Iterate on each cells which are not peers in 'c1'.
				int[] digits = grid.GetCandidates(c1).ToArray();
				foreach (int c2 in BivalueMap - new GridMap(c1))
				{
					if (c2 < c1 || grid.GetCandidateMask(c1) != grid.GetCandidateMask(c2))
					{
						continue;
					}

					var intersection = PeerMaps[c1] & PeerMaps[c2];
					if (!EmptyMap.Overlaps(intersection))
					{
						continue;
					}

					for (int region = 9; region < 27; region++)
					{
						if (region < 18 && (GetRegion(c1, Row) == region || GetRegion(c2, Row) == region)
							|| region >= 18 && (GetRegion(c1, Column) == region || GetRegion(c2, Column) == region))
						{
							continue;
						}

						for (int i = 0; i < 2; i++)
						{
							int digit = digits[i];
							var map = RegionMaps[region] & CandMaps[digit];
							if (map.Count != 2)
							{
								continue;
							}

							short mask = map.GetSubviewMask(region);
							int pos1 = mask.FindFirstSet(), pos2 = mask.GetNextSet(pos1);
							int bridgeStart = RegionCells[region][pos1], bridgeEnd = RegionCells[region][pos2];
							if (c1 == bridgeStart || c2 == bridgeStart || c1 == bridgeEnd || c2 == bridgeEnd)
							{
								continue;
							}

							static bool c(int c1, int c2) => (PeerMaps[c1] & PeerMaps[c2]).AllSetsAreInOneRegion(out _);
							if (!(c(bridgeStart, c1) && c(bridgeEnd, c2)) && !(c(bridgeStart, c2) && c(bridgeEnd, c1)))
							{
								continue;
							}

							// W-Wing found.
							int elimDigit = i == 0 ? digits[1] : digits[0];
							var elimMap = intersection & CandMaps[elimDigit];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							var conclusions = new List<Conclusion>();
							foreach (int offset in elimMap)
							{
								conclusions.Add(new Conclusion(Elimination, offset, elimDigit));
							}

							accumulator.Add(
								new WWingTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets: new[]
											{
												(1, c1 * 9 + elimDigit),
												(0, c1 * 9 + digit),
												(0, bridgeStart * 9 + digit),
												(0, bridgeEnd * 9 + digit),
												(0, c2 * 9 + digit),
												(1, c2 * 9 + elimDigit)
											},
											regionOffsets: new[] { (0, region) },
											links: null)
									},
									startCellOffset: c1,
									endCellOffset: c2,
									conjugatePair: new ConjugatePair(bridgeStart, bridgeEnd, digit)));
						}
					}
				}
			}
		}
	}
}
