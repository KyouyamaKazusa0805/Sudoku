using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	partial class QdpStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">(<see langword="in"/> parameter) The pair cells.</param>
		/// <param name="square">(<see langword="in"/> parameter) The square cells.</param>
		/// <param name="baseLine">(<see langword="in"/> parameter) The base line cells.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="otherDigitsMask">Other digits mask.</param>
		partial void CheckType1(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair, in GridMap square,
			in GridMap baseLine, in Pattern pattern, short comparer, short otherDigitsMask)
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
			foreach (int digit in mask)
			{
				conclusions.Add(new(Elimination, elimCell, digit));
			}

			var cellOffsets = (from cell in square | pair select new DrawingInfo(0, cell)).ToArray();
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int digit in comparer)
			{
				foreach (int cell in square & CandMaps[digit])
				{
					candidateOffsets.Add(new(1, cell * 9 + digit));
				}
			}
			int anotherCellInPair = (pair - map).First;
			foreach (int digit in grid.GetCandidateMask(anotherCellInPair))
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
							Regions = (
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select new DrawingInfo(0, pos + (isRow ? 9 : 18))
							).ToArray()
						}
					},
					pattern,
					elimCell * 9 + extraDigit));
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">(<see langword="in"/> parameter) The pair cells.</param>
		/// <param name="square">(<see langword="in"/> parameter) The square cells.</param>
		/// <param name="baseLine">(<see langword="in"/> parameter) The base line cells.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="otherDigitsMask">Other digits mask.</param>
		partial void CheckType2(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair,
			in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer, short otherDigitsMask)
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

			var cellOffsets = (from cell in square | pair select new DrawingInfo(0, cell)).ToArray();
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
				foreach (int digit in grid.GetCandidateMask(cell))
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
							Regions = (
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select new DrawingInfo(0, pos +(isRow ? 9 : 18))
							).ToArray()
						}
					},
					pattern,
					extraDigit));
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">(<see langword="in"/> parameter) The pair cells.</param>
		/// <param name="square">(<see langword="in"/> parameter) The square cells.</param>
		/// <param name="baseLine">(<see langword="in"/> parameter) The base line cells.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="otherDigitsMask">Other digits mask.</param>
		partial void CheckType3(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair,
			in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer, short otherDigitsMask)
		{
			foreach (int region in pair.CoveredRegions)
			{
				var allCellsMap = (RegionMaps[region] & EmptyMap) - pair;
				int[] allCells = allCellsMap.ToArray();
				for (int size = otherDigitsMask.PopCount() - 1; size < allCells.Length; size++)
				{
					foreach (int[] cells in allCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}

						if ((mask & comparer) != comparer || mask.PopCount() != size + 1)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask)
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

						var cellOffsets = (from cell in square | pair select new DrawingInfo(0, cell)).ToArray();
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
							foreach (int digit in grid.GetCandidateMask(cell))
							{
								candidateOffsets.Add(new((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidateMask(cell))
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
										Regions = (
											from pos
											in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
											select new DrawingInfo(0, pos + (isRow ? 9 : 18))
										).ToArray()
									}
								},
								pattern,
								mask,
								cells));
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">(<see langword="in"/> parameter) The pair cells.</param>
		/// <param name="square">(<see langword="in"/> parameter) The square cells.</param>
		/// <param name="baseLine">(<see langword="in"/> parameter) The base line cells.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		partial void CheckType4(
			IList<StepInfo> accumulator, bool isRow, in GridMap pair, in GridMap square,
			in GridMap baseLine, in Pattern pattern, short comparer)
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

					var cellOffsets = (from cell in square | pair select new DrawingInfo(0, cell)).ToArray();
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
									Regions = (
										from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
										select new DrawingInfo(0, pos + (isRow ? 9 : 18))
									).ToArray()
								}
							},
							pattern,
							new(pair, digit)));
				}
			}
		}

		/// <summary>
		/// Check locked type.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="isRow">Indicates whether the searching is for rows.</param>
		/// <param name="pair">(<see langword="in"/> parameter) The pair cells.</param>
		/// <param name="square">(<see langword="in"/> parameter) The square cells.</param>
		/// <param name="baseLine">(<see langword="in"/> parameter) The base line cells.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="comparer">The mask comparer.</param>
		partial void CheckLockedType(
			IList<StepInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair, in GridMap square,
			in GridMap baseLine, in Pattern pattern, short comparer)
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

			var cellOffsets = (from cell in square | pair select new DrawingInfo(0, cell)).ToArray();
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
				foreach (int digit in grid.GetCandidateMask(cell))
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
							Regions = (
								from pos in (isRow ? baseLine.RowMask : baseLine.ColumnMask).GetAllSets()
								select new DrawingInfo(0, pos + (isRow ? 9 : 18))
							).ToArray()
						}
					},
					pattern,
					candidates));
		}
	}
}
