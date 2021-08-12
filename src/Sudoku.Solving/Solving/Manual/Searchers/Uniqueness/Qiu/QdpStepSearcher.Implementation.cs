using System.Numerics;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	partial class QdpStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">The pair cells.</param>
		/// <param name="square">The square cells.</param>
		/// <param name="baseLine">The base line cells.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="otherDigitsMask">Other digits mask.</param>
		partial void CheckType1(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in Cells pair, in Cells square,
			in Cells baseLine, in Pattern pattern, short comparer, short otherDigitsMask)
		{
			if (otherDigitsMask == 0 || (otherDigitsMask & otherDigitsMask - 1) != 0)
			{
				return;
			}

			int extraDigit = TrailingZeroCount(otherDigitsMask);
			var map = pair & CandMaps[extraDigit];
			if (map.Count != 1)
			{
				return;
			}

			int elimCell = map[0];
			short mask = (short)(grid.GetCandidates(elimCell) & ~(1 << extraDigit));
			if (mask == 0)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int digit in mask)
			{
				conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
			}

			var cellsMap = square | pair;
			var cellOffsets = new DrawingInfo[cellsMap.Count];
			int i = 0;
			foreach (int cell in cellsMap)
			{
				cellOffsets[i++] = new(0, cell);
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int digit in comparer)
			{
				foreach (int cell in square & CandMaps[digit])
				{
					candidateOffsets.Add(new(1, cell * 9 + digit));
				}
			}
			int anotherCellInPair = (pair - map)[0];
			foreach (int digit in grid.GetCandidates(anotherCellInPair))
			{
				candidateOffsets.Add(new(0, anotherCellInPair * 9 + digit));
			}

			accumulator.Add(
				new QdpType1StepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = cellOffsets,
							Candidates = candidateOffsets,
							Regions =
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select new DrawingInfo(0, pos + (isRow ? 9 : 18))
						}
					},
					pattern,
					elimCell * 9 + extraDigit
				)
			);
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">The pair cells.</param>
		/// <param name="square">The square cells.</param>
		/// <param name="baseLine">The base line cells.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="otherDigitsMask">Other digits mask.</param>
		partial void CheckType2(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in Cells pair,
			in Cells square, in Cells baseLine, in Pattern pattern, short comparer, short otherDigitsMask)
		{
			if (otherDigitsMask == 0 || (otherDigitsMask & otherDigitsMask - 1) != 0)
			{
				return;
			}

			int extraDigit = TrailingZeroCount(otherDigitsMask);
			Cells map = pair & CandMaps[extraDigit], elimMap = map.PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
			}

			var cellsMap = square | pair;
			var cellOffsets = new DrawingInfo[cellsMap.Count];
			int i = 0;
			foreach (int cell in cellsMap)
			{
				cellOffsets[i++] = new(0, cell);
			}
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int digit in comparer)
			{
				foreach (int cell in square & CandMaps[digit])
				{
					candidateOffsets.Add(new(1, cell * 9 + digit));
				}
			}
			foreach (int cell in pair)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.Add(
				new QdpType2StepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = cellOffsets,
							Candidates = candidateOffsets,
							Regions =
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select new DrawingInfo(0, pos +(isRow ? 9 : 18))
						}
					},
					pattern,
					extraDigit
				)
			);
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">The pair cells.</param>
		/// <param name="square">The square cells.</param>
		/// <param name="baseLine">The base line cells.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="otherDigitsMask">Other digits mask.</param>
		partial void CheckType3(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in Cells pair,
			in Cells square, in Cells baseLine, in Pattern pattern, short comparer, short otherDigitsMask)
		{
			foreach (int region in pair.CoveredRegions)
			{
				var allCellsMap = (RegionMaps[region] & EmptyMap) - pair;
				int[] allCells = allCellsMap.ToArray();
				for (
					int size = PopCount((uint)otherDigitsMask) - 1, length = allCells.Length;
					size < length;
					size++
				)
				{
					foreach (int[] cells in allCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidates(cell);
						}

						if ((mask & comparer) != comparer || PopCount((uint)mask) != size + 1)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask)
						{
							foreach (int cell in allCellsMap - cells & CandMaps[digit])
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var cellsMap = square | pair;
						var cellOffsets = new DrawingInfo[cellsMap.Count];
						int i = 0;
						foreach (int cell in cellsMap)
						{
							cellOffsets[i++] = new(0, cell);
						}
						var candidateOffsets = new List<DrawingInfo>();
						foreach (int digit in comparer)
						{
							foreach (int cell in square & CandMaps[digit])
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}
						foreach (int cell in pair)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new QdpType3StepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Cells = cellOffsets,
										Candidates = candidateOffsets,
										Regions =
											from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
											select new DrawingInfo(0, pos + (isRow ? 9 : 18))
									}
								},
								pattern,
								mask,
								cells
							)
						);
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">The pair cells.</param>
		/// <param name="square">The square cells.</param>
		/// <param name="baseLine">The base line cells.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		partial void CheckType4(
			IList<StepInfo> accumulator, bool isRow, in Cells pair, in Cells square,
			in Cells baseLine, in Pattern pattern, short comparer)
		{
			foreach (int region in pair.CoveredRegions)
			{
				foreach (int digit in comparer)
				{
					if ((CandMaps[digit] & RegionMaps[region]) != pair)
					{
						continue;
					}

					short otherDigitsMask = (short)(comparer & ~(1 << digit));
					bool flag = false;
					foreach (int d in otherDigitsMask)
					{
						if (!(ValueMaps[d] & RegionMaps[region]).IsEmpty
							|| (RegionMaps[region] & CandMaps[d]) != square)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}

					int elimDigit = TrailingZeroCount(comparer & ~(1 << digit));
					var elimMap = pair & CandMaps[elimDigit];
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
					}

					var cellsMap = square | pair;
					var cellOffsets = new DrawingInfo[cellsMap.Count];
					int i = 0;
					foreach (int cell in cellsMap)
					{
						cellOffsets[i++] = new(0, cell);
					}
					var candidateOffsets = new List<DrawingInfo>();
					foreach (int d in comparer)
					{
						foreach (int cell in square & CandMaps[d])
						{
							candidateOffsets.Add(new(1, cell * 9 + d));
						}
					}
					foreach (int cell in pair)
					{
						candidateOffsets.Add(new(1, cell * 9 + digit));
					}

					accumulator.Add(
						new QdpType4StepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = cellOffsets,
									Candidates = candidateOffsets,
									Regions =
										from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
										select new DrawingInfo(0, pos + (isRow ? 9 : 18))
								}
							},
							pattern,
							new(pair, digit)
						)
					);
				}
			}
		}

		/// <summary>
		/// Check locked type.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">The pair cells.</param>
		/// <param name="square">The square cells.</param>
		/// <param name="baseLine">The base line cells.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		partial void CheckLockedType(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in Cells pair, in Cells square,
			in Cells baseLine, in Pattern pattern, short comparer)
		{
			// Firstly, we should check the cells in the block that the square cells lying on.
			int block = TrailingZeroCount(square.BlockMask);
			var otherCellsMap = (RegionMaps[block] & EmptyMap) - square;
			var tempMap = Cells.Empty;
			var pairDigits = comparer.GetAllSets();

			bool flag = false;
			foreach (int digit in pairDigits)
			{
				if (!(ValueMaps[digit] & RegionMaps[block]).IsEmpty)
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
			if (otherCellsMap is { IsEmpty: true } or { Count: > 5 })
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

			var elimMap = new Candidates(candidates).PeerIntersection;
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int candidate in elimMap)
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

			var cellsMap = square | pair;
			var cellOffsets = new DrawingInfo[cellsMap.Count];
			int i = 0;
			foreach (int cell in cellsMap)
			{
				cellOffsets[i++] = new(0, cell);
			}
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int d in comparer)
			{
				foreach (int cell in square & CandMaps[d])
				{
					candidateOffsets.Add(new(1, cell * 9 + d));
				}
			}
			foreach (int cell in pair)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(0, cell * 9 + digit));
				}
			}
			foreach (int candidate in candidates)
			{
				candidateOffsets.Add(new(2, candidate));
			}

			accumulator.Add(
				new QdpLockedTypeStepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = cellOffsets,
							Candidates = candidateOffsets,
							Regions =
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select new DrawingInfo(0, pos + (isRow ? 9 : 18))
						}
					},
					pattern,
					candidates
				)
			);
		}
	}
}
