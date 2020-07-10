using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Encapsulates a <b>Qiu's deadly pattern</b> technique searcher.
	/// <code>
	/// .-------.-------.-------.<br/>
	/// | . . . | . . . | . . . |<br/>
	/// | . . . | . . . | . . . |<br/>
	/// | P P . | . . . | . . . |<br/>
	/// :-------+-------+-------:<br/>
	/// | S S B | B B B | B B B |<br/>
	/// | S S B | B B B | B B B |<br/>
	/// | . . . | . . . | . . . |<br/>
	/// :-------+-------+-------:<br/>
	/// | . . . | . . . | . . . |<br/>
	/// | . . . | . . . | . . . |<br/>
	/// | . . . | . . . | . . . |<br/>
	/// '-------'-------'-------'
	/// </code>
	/// Where:
	/// <list type="table">
	/// <item><term>P</term><description>Pair Cells.</description></item>
	/// <item><term>S</term><description>Square Cells.</description></item>
	/// <item><term>B</term><description>Base Line Cells.</description></item>
	/// </list>
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.QdpType1))]
	public sealed partial class QdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		private static readonly Pattern[] Patterns = new Pattern[972];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 58;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int i = 0, length = Patterns.Length; i < length; i++)
			{
				var pattern = Patterns[i];
				bool isRow = i < length >> 1;
				var (pair, square, baseLine) = pattern;

				// To check whether both two pair cells are empty.
				if (!EmptyMap[pair.SetAt(0)] || !EmptyMap[pair.SetAt(1)])
				{
					continue;
				}

				// Step 1: To determine whether the distinction degree of base line is 1.
				short appearedDigitsMask = 0, distinctionMask = 0;
				int appearedParts = 0;
				for (int j = 0, region = isRow ? 18 : 9; j < 9; j++, region++)
				{
					var regionMap = RegionMaps[region];
					var tempMap = baseLine & regionMap;
					if (tempMap.IsNotEmpty)
					{
						f(tempMap);
					}
					else
					{
						// Don't forget to record the square cells.
						var squareMap = square & regionMap;
						if (squareMap.IsNotEmpty)
						{
							f(squareMap);
						}
						else
						{
							continue;
						}
					}

					void f(GridMap map)
					{
						bool flag = false;
						int c1 = map.SetAt(0), c2 = map.SetAt(1);
						if (!EmptyMap[c1])
						{
							int d1 = grid[c1];
							distinctionMask ^= (short)(1 << d1);
							appearedDigitsMask |= (short)(1 << d1);

							flag = true;
						}
						if (!EmptyMap[c2])
						{
							int d2 = grid[c2];
							distinctionMask ^= (short)(1 << d2);
							appearedDigitsMask |= (short)(1 << d2);

							flag = true;
						}

						appearedParts += flag ? 1 : 0;
					}
				}

				if (!distinctionMask.IsPowerOfTwo() || appearedParts != appearedDigitsMask.CountSet())
				{
					continue;
				}

				short pairMask = 0;
				foreach (int cell in pair)
				{
					pairMask |= grid.GetCandidateMask(cell);
				}

				// Iterate on each combination.
				for (int size = 2, count = pairMask.CountSet(); size < count; size++)
				{
					foreach (int[] digits in pairMask.GetAllSets().ToArray().GetSubsets(size))
					{
						// Step 2: To determine whether the digits in pair cells
						// will only appears in square cells.
						var tempMap = GridMap.Empty;
						foreach (int digit in digits)
						{
							tempMap |= CandMaps[digit];
						}
						var appearingMap = tempMap & square;
						if (appearingMap.Count != 4)
						{
							continue;
						}

						bool flag = false;
						foreach (int digit in digits)
						{
							if (!square.Overlaps(CandMaps[digit]))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}

						short comparer = 0;
						foreach (int digit in digits)
						{
							comparer |= (short)(1 << digit);
						}
						short otherDigitsMask = (short)(pairMask & ~comparer);
						if (appearingMap == (tempMap & RegionMaps[square.BlockMask.FindFirstSet()]))
						{
							// Qdp forms.
							// Now check each type.
							CheckType1(accumulator, grid, isRow, pair, square, baseLine, pattern, comparer, otherDigitsMask);
							CheckType2(accumulator, grid, isRow, pair, square, baseLine, pattern, comparer, otherDigitsMask);
							CheckType3(accumulator, grid, isRow, pair, square, baseLine, pattern, comparer, otherDigitsMask);
						}
					}
				}

				CheckType4(accumulator, isRow, pair, square, baseLine, pattern, pairMask);
				CheckLockedType(accumulator, grid, isRow, pair, square, baseLine, pattern, pairMask);
			}
		}

		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isRow, GridMap pair, GridMap square,
			GridMap baseLine, Pattern pattern, short comparer, short otherDigitsMask)
		{
			if (!otherDigitsMask.IsPowerOfTwo())
			{
				return;
			}

			int extraDigit = otherDigitsMask.FindFirstSet();
			var map = pair & CandMaps[extraDigit];
			if (map.Count != 1)
			{
				return;
			}

			int elimCell = map.SetAt(0);
			short mask = (short)(grid.GetCandidateMask(elimCell) & ~(1 << extraDigit));
			if (mask == 0)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int digit in mask.GetAllSets())
			{
				conclusions.Add(new Conclusion(Elimination, elimCell, digit));
			}

			var cellOffsets = new List<(int, int)>(from cell in square | pair select (0, cell));
			var candidateOffsets = new List<(int, int)>();
			foreach (int digit in comparer.GetAllSets())
			{
				foreach (int cell in square & CandMaps[digit])
				{
					candidateOffsets.Add((1, cell * 9 + digit));
				}
			}
			int anotherCellInPair = (pair - map).SetAt(0);
			foreach (int digit in grid.GetCandidates(anotherCellInPair))
			{
				candidateOffsets.Add((0, anotherCellInPair * 9 + digit));
			}

			accumulator.Add(
				new QdpType1TechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets,
							candidateOffsets,
							regionOffsets:
								new List<(int, int)>(
									from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
									select (0, pos + (isRow ? 9 : 18))),
							links: null)
					},
					pattern,
					candidate: elimCell * 9 + extraDigit));
		}

		private void CheckType2(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isRow, GridMap pair, GridMap square,
			GridMap baseLine, Pattern pattern, short comparer, short otherDigitsMask)
		{
			if (!otherDigitsMask.IsPowerOfTwo())
			{
				return;
			}

			int extraDigit = otherDigitsMask.FindFirstSet();
			var map = pair & CandMaps[extraDigit];
			var elimMap = map.PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
			}

			var cellOffsets = new List<(int, int)>(from cell in square | pair select (0, cell));
			var candidateOffsets = new List<(int, int)>();
			foreach (int digit in comparer.GetAllSets())
			{
				foreach (int cell in square & CandMaps[digit])
				{
					candidateOffsets.Add((1, cell * 9 + digit));
				}
			}
			foreach (int cell in pair)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.Add(
				new QdpType2TechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets,
							candidateOffsets,
							regionOffsets:
								new List<(int, int)>(
									from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
									select (0, pos + (isRow ? 9 : 18))),
							links: null)
					},
					pattern,
					extraDigit));
		}

		private void CheckType3(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isRow, GridMap pair, GridMap square,
			GridMap baseLine, Pattern pattern, short comparer, short otherDigitsMask)
		{
			foreach (int region in pair.CoveredRegions)
			{
				var allCellsMap = (RegionMaps[region] & EmptyMap) - pair;
				int[] allCells = allCellsMap.ToArray();
				for (int size = otherDigitsMask.CountSet() - 1; size < allCells.Length; size++)
				{
					foreach (int[] cells in allCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}

						if ((mask & comparer) != comparer || mask.CountSet() != size + 1)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask.GetAllSets())
						{
							foreach (int cell in allCellsMap - new GridMap(cells) & CandMaps[digit])
							{
								conclusions.Add(new Conclusion(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var cellOffsets = new List<(int, int)>(from cell in square | pair select (0, cell));
						var candidateOffsets = new List<(int, int)>();
						foreach (int digit in comparer.GetAllSets())
						{
							foreach (int cell in square & CandMaps[digit])
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
						foreach (int cell in pair)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
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
							new QdpType3TechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets,
										candidateOffsets,
										regionOffsets:
											new List<(int, int)>(
												from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
												select (0, pos + (isRow ? 9 : 18))),
										links: null)
								},
								pattern,
								extraDigitsMask: mask,
								extraCells: cells));
					}
				}
			}
		}

		private void CheckType4(
			IBag<TechniqueInfo> accumulator, bool isRow, GridMap pair, GridMap square,
			GridMap baseLine, Pattern pattern, short comparer)
		{
			foreach (int region in pair.CoveredRegions)
			{
				foreach (int digit in comparer.GetAllSets())
				{
					if ((CandMaps[digit] & RegionMaps[region]) != pair)
					{
						continue;
					}

					short otherDigitsMask = (short)(comparer & ~(1 << digit));
					bool flag = false;
					foreach (int d in otherDigitsMask.GetAllSets())
					{
						if (ValueMaps[d].Overlaps(RegionMaps[region]) || (RegionMaps[region] & CandMaps[d]) != square)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}

					int elimDigit = (comparer & ~(1 << digit)).FindFirstSet();
					var elimMap = pair & CandMaps[elimDigit];
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
					}

					var cellOffsets = new List<(int, int)>(from cell in square | pair select (0, cell));
					var candidateOffsets = new List<(int, int)>();
					foreach (int d in comparer.GetAllSets())
					{
						foreach (int cell in square & CandMaps[d])
						{
							candidateOffsets.Add((1, cell * 9 + d));
						}
					}
					foreach (int cell in pair)
					{
						candidateOffsets.Add((1, cell * 9 + digit));
					}

					accumulator.Add(
						new QdpType4TechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets,
									candidateOffsets,
									regionOffsets:
										new List<(int, int)>(
											from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
											select (0, pos + (isRow ? 9 : 18))),
									links: null)
							},
							pattern,
							conjugatePair: new ConjugatePair(pair, digit)));
				}
			}
		}

		private void CheckLockedType(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isRow, GridMap pair, GridMap square,
			GridMap baseLine, Pattern pattern, short comparer)
		{
			// Firstly, we should check the cells in the block that the square cells lying on.
			int block = square.BlockMask.FindFirstSet();
			var otherCellsMap = (RegionMaps[block] & EmptyMap) - square;
			var tempMap = GridMap.Empty;
			var pairDigits = comparer.GetAllSets();

			bool flag = false;
			foreach (int digit in pairDigits)
			{
				if (ValueMaps[digit].Overlaps(RegionMaps[block]))
				{
					flag = true;
					break;
				}

				tempMap |= CandMaps[digit];
			}
			if (flag)
			{
				return;
			}

			otherCellsMap &= tempMap;
			if (otherCellsMap.IsEmpty || otherCellsMap.Count > 5)
			{
				return;
			}

			// May be in one region or span two regions.
			// Now we check for this case.
			var candidates = new List<int>();
			foreach (int cell in otherCellsMap)
			{
				foreach (int digit in pairDigits)
				{
					if (grid.Exists(cell, digit) is true)
					{
						candidates.Add(cell * 9 + digit);
					}
				}
			}

			var elimMap = SudokuMap.CreateInstance(candidates);
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int candidate in elimMap)
			{
				if (grid.Exists(candidate / 9, candidate % 9) is true)
				{
					conclusions.Add(new Conclusion(Elimination, candidate));
				}
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var cellOffsets = new List<(int, int)>(from cell in square | pair select (0, cell));
			var candidateOffsets = new List<(int, int)>();
			foreach (int d in comparer.GetAllSets())
			{
				foreach (int cell in square & CandMaps[d])
				{
					candidateOffsets.Add((1, cell * 9 + d));
				}
			}
			foreach (int cell in pair)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			foreach (int candidate in candidates)
			{
				candidateOffsets.Add((2, candidate));
			}

			accumulator.Add(
				new QdpLockedTypeTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets,
							candidateOffsets,
							regionOffsets:
								new List<(int, int)>(
									from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
									select (0, pos + (isRow ? 9 : 18))),
							links: null)
					},
					pattern,
					candidates));
		}
	}
}
