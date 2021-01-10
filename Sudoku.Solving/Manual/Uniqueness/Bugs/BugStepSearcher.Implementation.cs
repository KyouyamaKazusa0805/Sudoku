using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	partial class BugStepSearcher
	{
		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		partial void CheckType2(IList<StepInfo> accumulator, IReadOnlyList<int> trueCandidates)
		{
			var cells = (stackalloc int[trueCandidates.Count]);
			int i = 0;
			foreach (int candidate in trueCandidates)
			{
				cells[i++] = candidate / 9;
			}
			var map = new Cells(cells).PeerIntersection;
			if (map.IsEmpty)
			{
				return;
			}

			int digit = trueCandidates[0] % 9;
			var elimMap = map & CandMaps[digit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(ConclusionType.Elimination, cell, digit));
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int candidate in trueCandidates)
			{
				candidateOffsets.Add(new(0, candidate));
			}

			// BUG type 2.
			accumulator.Add(
				new BugType2StepInfo(
					conclusions,
					new View[] { new() { Candidates = candidateOffsets } },
					digit,
					cells.ToArray()));
		}

		/// <summary>
		/// Check type 3 (with naked subsets).
		/// </summary>
		/// <param name="accumulator">The result.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		partial void CheckType3Naked(
			IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates)
		{
			// Check whether all true candidates lie in a same region.
			var map = new Cells(from c in trueCandidates group c by c / 9 into z select z.Key);
			if (!map.InOneRegion)
			{
				return;
			}

			// Get the digit mask.
			short digitsMask = 0;
			foreach (int candidate in trueCandidates)
			{
				digitsMask |= (short)(1 << candidate % 9);
			}

			// Iterate on each region that the true candidates lying on.
			foreach (int region in map.CoveredRegions)
			{
				var regionMap = RegionMaps[region];
				var otherCellsMap = (regionMap & EmptyMap) - map;
				if (otherCellsMap.IsEmpty)
				{
					continue;
				}

				// Iterate on each size.
				int[] otherCells = otherCellsMap.ToArray();
				for (int size = 1, length = otherCells.Length; size < length; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = digitsMask;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidates(cell);
						}
						if (mask.PopCount() != size + 1)
						{
							continue;
						}

						var elimMap = (regionMap - cells - map) & EmptyMap;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int cell in elimMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								if (mask.ContainsBit(digit))
								{
									conclusions.Add(new(ConclusionType.Elimination, cell, digit));
								}
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cand in trueCandidates)
						{
							candidateOffsets.Add(new(0, cand));
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new BugType3StepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) }
									}
								},
								trueCandidates,
								digitsMask.GetAllSets().ToArray(),
								cells,
								true));
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The result.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		partial void CheckType4(
			IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates)
		{
			// Conjugate pairs should lie in two cells.
			var candsGroupByCell = from candidate in trueCandidates group candidate by candidate / 9;
			if (candsGroupByCell.Take(3).Count() != 2)
			{
				return;
			}

			// Check two cell has same region.
			var cells = new List<int>();
			foreach (var candGroupByCell in candsGroupByCell)
			{
				cells.Add(candGroupByCell.Key);
			}

			int regions = new Cells(cells).CoveredRegions;
			if (regions != 0)
			{
				return;
			}

			// Check for each region.
			foreach (int region in regions)
			{
				// Add up all digits.
				var digits = new HashSet<int>();
				foreach (var candGroupByCell in candsGroupByCell)
				{
					foreach (int cand in candGroupByCell)
					{
						digits.Add(cand % 9);
					}
				}

				// Check whether exists a conjugate pair in this region.
				for (int conjuagtePairDigit = 0; conjuagtePairDigit < 9; conjuagtePairDigit++)
				{
					// Check whether forms a conjugate pair.
					short mask = (RegionMaps[region] & CandMaps[conjuagtePairDigit]).GetSubviewMask(region);
					if (mask.PopCount() != 2)
					{
						continue;
					}

					// Check whether the conjugate pair lies in current two cells.
					int c1 = RegionCells[region][mask.SetAt(0)];
					int c2 = RegionCells[region][mask.SetAt(1)];
					if (c1 != cells[0] || c2 != cells[1])
					{
						continue;
					}

					// Check whether all digits contain that digit.
					if (digits.Contains(conjuagtePairDigit))
					{
						continue;
					}

					// BUG type 4 found.
					// Now add up all eliminations.
					var conclusions = new List<Conclusion>();
					foreach (var candGroupByCell in candsGroupByCell)
					{
						int cell = candGroupByCell.Key;
						short digitMask = 0;
						foreach (int cand in candGroupByCell)
						{
							digitMask |= (short)(1 << cand % 9);
						}

						// Bitwise not.
						foreach (int d in digitMask = (short)(~digitMask & SudokuGrid.MaxCandidatesMask))
						{
							if (conjuagtePairDigit == d || grid.Exists(cell, d) is not true)
							{
								continue;
							}

							conclusions.Add(new(ConclusionType.Elimination, cell, d));
						}
					}

					// Check eliminations.
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new DrawingInfo[trueCandidates.Count + 2];
					int i = 0;
					foreach (int candidate in trueCandidates)
					{
						candidateOffsets[i++] = new(0, candidate);
					}
					candidateOffsets[^2] = new(1, c1 * 9 + conjuagtePairDigit);
					candidateOffsets[^1] = new(1, c2 * 9 + conjuagtePairDigit);

					// BUG type 4.
					accumulator.Add(
						new BugType4StepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, region) }
								}
							},
							digits.ToArray(),
							cells,
							new(c1, c2, conjuagtePairDigit)));
				}
			}
		}

		/// <summary>
		/// Check BUG + n.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		partial void CheckMultiple(
			IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates)
		{
			if (trueCandidates.Count > 18)
			{
				return;
			}

			var map = new Candidates(trueCandidates).PeerIntersection;
			if (map.IsEmpty)
			{
				return;
			}

			// BUG + n found.
			// Check eliminations.
			var conclusions = new List<Conclusion>();
			foreach (int candidate in map)
			{
				if (grid.Exists(candidate / 9, candidate % 9) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, candidate));
				}
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new DrawingInfo[trueCandidates.Count];
			int i = 0;
			foreach (int candidate in trueCandidates)
			{
				candidateOffsets[i++] = new(0, candidate);
			}

			// BUG + n.
			accumulator.Add(
				new BugMultipleStepInfo(
					conclusions,
					new View[] { new() { Candidates = candidateOffsets } },
					trueCandidates));
		}

		/// <summary>
		/// Check BUG-XZ.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		partial void CheckXz(IList<StepInfo> accumulator, in SudokuGrid grid, IReadOnlyList<int> trueCandidates)
		{
			if (trueCandidates.Count > 2)
			{
				return;
			}

			int cand1 = trueCandidates[0], cand2 = trueCandidates[1];
			int c1 = cand1 / 9, c2 = cand2 / 9, d1 = cand1 % 9, d2 = cand2 % 9;
			short mask = (short)(1 << d1 | 1 << d2);
			foreach (int cell in (PeerMaps[c1] ^ PeerMaps[c2]) & BivalueMap)
			{
				if (grid.GetCandidates(cell) != mask)
				{
					continue;
				}

				// BUG-XZ found.
				var conclusions = new List<Conclusion>();
				bool condition = new Cells { c1, cell }.InOneRegion;
				int anotherCell = condition ? c2 : c1;
				int anotherDigit = condition ? d2 : d1;
				foreach (int peer in new Cells { cell, anotherCell }.PeerIntersection)
				{
					if (grid.Exists(peer, anotherDigit) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, peer, anotherDigit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new DrawingInfo[trueCandidates.Count];
				int i = 0;
				foreach (int candidate in trueCandidates)
				{
					candidateOffsets[i++] = new(0, candidate);
				}

				accumulator.Add(
					new BugXzStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Cells = new DrawingInfo[] { new(0, cell) },
								Candidates = candidateOffsets
							}
						},
						mask,
						new[] { c1, c2 },
						cell));
			}
		}


		/// <summary>
		/// Check whether all candidates in the list has same digit value.
		/// </summary>
		/// <param name="list">The list of all true candidates.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static unsafe bool CheckSingleDigit(IReadOnlyList<int> list)
		{
			int i = 0, comparer;
			foreach (int cand in list)
			{
				if (i++ == 0)
				{
					comparer = cand % 9;
					continue;
				}

				if (*&comparer != cand % 9)
				{
					return false;
				}
			}

			return true;
		}
	}
}
