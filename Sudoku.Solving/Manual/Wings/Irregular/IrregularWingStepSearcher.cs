using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates an <b>irregular wing</b> technique searcher.
	/// </summary>
	public sealed class IrregularWingStepSearcher : StepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(7, nameof(TechniqueCode.WWing))
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
				return;
			}

			// Iterate on each cells.
			for (int c1 = 0; c1 < 72; c1++)
			{
				if (!BivalueMap.Contains(c1) || !EmptyMap.Contains(c1))
				{
					continue;
				}

				// Iterate on each cells which are not peers in 'c1'.
				var digits = grid.GetCandidates(c1).GetAllSets();
				foreach (int c2 in BivalueMap - new Cells(c1))
				{
					if (c2 < c1 || grid.GetCandidates(c1) != grid.GetCandidates(c2))
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
						if (region < 18 && (
							RegionLabel.Row.ToRegion(c1) == region
							|| RegionLabel.Row.ToRegion(c2) == region)
							|| region >= 18 && (
							RegionLabel.Column.ToRegion(c1) == region
							|| RegionLabel.Column.ToRegion(c2) == region))
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

							static bool c(int c1, int c2) => (PeerMaps[c1] & PeerMaps[c2]).InOneRegion;
							if (!(c(bridgeStart, c1) && c(bridgeEnd, c2))
								&& !(c(bridgeStart, c2) && c(bridgeEnd, c1)))
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
								conclusions.Add(new(ConclusionType.Elimination, offset, elimDigit));
							}

							accumulator.Add(
								new WWingStepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Candidates = new DrawingInfo[]
											{
												new(1, c1 * 9 + elimDigit),
												new(0, c1 * 9 + digit),
												new(0, bridgeStart * 9 + digit),
												new(0, bridgeEnd * 9 + digit),
												new(0, c2 * 9 + digit),
												new(1, c2 * 9 + elimDigit)
											},
											Regions = new DrawingInfo[] { new(0, region) }
										}
									},
									c1,
									c2,
									new(bridgeStart, bridgeEnd, digit)));
						}
					}
				}
			}
		}
	}
}
