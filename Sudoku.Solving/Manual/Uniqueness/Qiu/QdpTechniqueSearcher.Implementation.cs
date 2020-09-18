using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	partial class QdpTechniqueSearcher
	{
		partial void CheckType1(
			IList<TechniqueInfo> accumulator, Grid grid, bool isRow, GridMap pair, GridMap square,
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

			int elimCell = map.First;
			short mask = (short)(grid.GetCandidateMask(elimCell) & ~(1 << extraDigit));
			if (mask == 0)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int digit in mask.GetAllSets())
			{
				conclusions.Add(new(Elimination, elimCell, digit));
			}

			var cellOffsets = (from cell in square | pair select (0, cell)).ToArray();
			var candidateOffsets = new List<(int, int)>();
			foreach (int digit in comparer.GetAllSets())
			{
				foreach (int cell in square & CandMaps[digit])
				{
					candidateOffsets.Add((1, cell * 9 + digit));
				}
			}
			int anotherCellInPair = (pair - map).First;
			foreach (int digit in grid.GetCandidates(anotherCellInPair))
			{
				candidateOffsets.Add((0, anotherCellInPair * 9 + digit));
			}

			accumulator.Add(
				new QdpType1TechniqueInfo(
					conclusions,
					new View[]
					{
						new(
							cellOffsets,
							candidateOffsets,
							(
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select (0, pos + (isRow ? 9 : 18))
							).ToArray(),
							null)
					},
					pattern,
					elimCell * 9 + extraDigit));
		}

		partial void CheckType2(
			IList<TechniqueInfo> accumulator, Grid grid, bool isRow, GridMap pair, GridMap square,
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
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			var cellOffsets = (from cell in square | pair select (0, cell)).ToList();
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
					new View[]
					{
						new(
							cellOffsets,
							candidateOffsets,
							(
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select (0, pos +(isRow ? 9 : 18))
							).ToArray(),
							null)
					},
					pattern,
					extraDigit));
		}

		partial void CheckType3(
			IList<TechniqueInfo> accumulator, Grid grid, bool isRow, GridMap pair, GridMap square,
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
							foreach (int cell in allCellsMap - cells & CandMaps[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var cellOffsets = (from cell in square | pair select (0, cell)).ToList();
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
								new View[]
								{
									new(
										cellOffsets,
										candidateOffsets,
										(
											from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
											select (0, pos + (isRow ? 9 : 18))
										).ToArray(),
										null)
								},
								pattern,
								mask,
								cells));
					}
				}
			}
		}

		partial void CheckType4(
			IList<TechniqueInfo> accumulator, bool isRow, GridMap pair, GridMap square,
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
						conclusions.Add(new(Elimination, cell, elimDigit));
					}

					var cellOffsets = (from cell in square | pair select (0, cell)).ToList();
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
							new View[]
							{
								new(
									cellOffsets,
									candidateOffsets,
									(
										from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
										select (0, pos + (isRow ? 9 : 18))
									).ToArray(),
									null)
							},
							pattern,
							new(pair, digit)));
				}
			}
		}

		partial void CheckLockedType(
			IList<TechniqueInfo> accumulator, Grid grid, bool isRow, GridMap pair, GridMap square,
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

			var elimMap = new SudokuMap(candidates).PeerIntersection;
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int candidate in elimMap)
			{
				if (grid.Exists(candidate / 9, candidate % 9) is true)
				{
					conclusions.Add(new(Elimination, candidate));
				}
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var cellOffsets = (from cell in square | pair select (0, cell)).ToList();
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
					new View[]
					{
						new(
							cellOffsets,
							candidateOffsets,
							(
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select (0, pos + (isRow ? 9 : 18))
							).ToArray(),
							null)
					},
					pattern,
					candidates));
		}
	}
}
