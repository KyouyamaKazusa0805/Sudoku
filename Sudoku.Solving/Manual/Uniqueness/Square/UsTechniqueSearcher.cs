using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.UsType1))]
	public sealed partial class UsTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the patterns.
		/// </summary>
		private static readonly GridMap[] Patterns = new GridMap[162];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 53;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (var pattern in Patterns)
			{
				if ((EmptyMap | pattern) != EmptyMap)
				{
					continue;
				}

				short mask = 0;
				foreach (int cell in pattern)
				{
					mask |= grid.GetCandidateMask(cell);
				}

				CheckType1(accumulator, grid, pattern, mask);
				CheckType2(accumulator, pattern, mask);
				CheckType3(accumulator, grid, pattern, mask);
				CheckType4(accumulator, grid, pattern, mask);
			}
		}

		private void CheckType1(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask)
		{
			if (mask.CountSet() != 5)
			{
				return;
			}

			foreach (int[] digits in mask.GetAllSets().ToArray().GetCombinations(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = (mask & ~digitsMask).FindFirstSet();
				var extraDigitMap = CandMaps[extraDigit] & pattern;
				if (extraDigitMap.Count != 1)
				{
					continue;
				}

				int elimCell = extraDigitMap.SetAt(0);
				short cellMask = grid.GetCandidateMask(elimCell);
				short elimMask = (short)(cellMask & ~(1 << extraDigit));
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask.GetAllSets())
				{
					conclusions.Add(new Conclusion(Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in digits)
				{
					foreach (int cell in pattern - elimCell & CandMaps[digit])
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new UsType1TechniqueInfo(
						conclusions,
						views: new[] { new View(candidateOffsets) },
						cells: pattern,
						digitsMask,
						candidate: elimCell * 9 + extraDigit));
			}
		}

		private void CheckType2(IBag<TechniqueInfo> accumulator, GridMap pattern, short mask)
		{
			if (mask.CountSet() != 5)
			{
				return;
			}

			foreach (int[] digits in mask.GetAllSets().ToArray().GetCombinations(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = (mask & ~digitsMask).FindFirstSet();
				var elimMap = (CandMaps[extraDigit] & pattern).PeerIntersection & CandMaps[extraDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in digits)
				{
					foreach (int cell in CandMaps[digit] & pattern)
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}
				foreach (int cell in CandMaps[extraDigit] & pattern)
				{
					candidateOffsets.Add((1, cell * 9 + extraDigit));
				}

				accumulator.Add(
					new UsType2TechniqueInfo(
						conclusions,
						views: new[] { new View(candidateOffsets) },
						cells: pattern,
						digitsMask,
						extraDigit));
			}
		}

		private void CheckType3(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask)
		{
			foreach (int[] digits in mask.GetAllSets().ToArray().GetCombinations(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				short extraDigitsMask = (short)(mask & ~digitsMask);
				var tempMap = GridMap.Empty;
				foreach (int digit in extraDigitsMask.GetAllSets())
				{
					tempMap |= CandMaps[digit];
				}
				if (tempMap.AllSetsAreInOneRegion(out _))
				{
					continue;
				}

				foreach (int region in tempMap.CoveredRegions)
				{
					int[] allCells = ((RegionMaps[region] & EmptyMap) - pattern).ToArray();
					for (int size = extraDigitsMask.CountSet() - 1, count = allCells.Length; size < count; size++)
					{
						foreach (int[] cells in allCells.GetCombinations(size))
						{
							short tempMask = 0;
							foreach (int cell in cells)
							{
								tempMask |= grid.GetCandidateMask(cell);
							}

							if (tempMask.CountSet() != size + 1 || (tempMask & extraDigitsMask) != extraDigitsMask)
							{
								continue;
							}

							var cellsMap = new GridMap(cells);
							var conclusions = new List<Conclusion>();
							foreach (int digit in tempMask.GetAllSets())
							{
								foreach (int cell in (allCells - cellsMap) & CandMaps[digit])
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in pattern)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
								}
							}
							foreach (int cell in cells)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add((1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new UsType3TechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: new[] { (0, region) },
											links: null)
									},
									cells: pattern,
									digitsMask,
									extraDigitsMask,
									extraCells: cells));
						}
					}
				}
			}
		}

		private void CheckType4(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap pattern, short mask)
		{
			foreach (int[] digits in mask.GetAllSets().ToArray().GetCombinations(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				short extraDigitsMask = (short)(mask & ~digitsMask);
				var tempMap = GridMap.Empty;
				foreach (int digit in extraDigitsMask.GetAllSets())
				{
					tempMap |= CandMaps[digit];
				}
				if (tempMap.AllSetsAreInOneRegion(out _))
				{
					continue;
				}

				foreach (int region in tempMap.CoveredRegions)
				{
					int d1 = -1, d2 = -1, count = 0;
					var compareMap = RegionMaps[region] & pattern;
					foreach (int digit in digits)
					{
						if ((compareMap | RegionMaps[region] & CandMaps[digit]) == compareMap)
						{
							switch (count++)
							{
								case 0:
								{
									d1 = digit;

									break;
								}
								case 1:
								{
									d2 = digit;

									goto Label_Z;
								}
							}
						}
					}

				Label_Z:
					short comparer = (short)(1 << d1 | 1 << d2);
					short otherDigitsMask = (short)(digitsMask & ~comparer);
					var conclusions = new List<Conclusion>();
					foreach (int digit in otherDigitsMask.GetAllSets())
					{
						foreach (int cell in compareMap & CandMaps[digit])
						{
							conclusions.Add(new Conclusion(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in pattern - compareMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((0, cell * 9 + digit));
						}
					}
					foreach (int cell in compareMap & CandMaps[d1])
					{
						candidateOffsets.Add((1, cell * 9 + d1));
					}
					foreach (int cell in compareMap & CandMaps[d2])
					{
						candidateOffsets.Add((1, cell * 9 + d2));
					}

					accumulator.Add(
						new UsType4TechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							cells: pattern,
							digitsMask,
							d1,
							d2,
							conjugateRegion: compareMap));
				}
			}
		}
	}
}
