using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Encapsulates a <b>Qiu's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Qiu's Deadly Pattern")]
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
				for (int j = 0, region = isRow ? 18 : 9; j < 9; j++, region++)
				{
					var tempMap = baseLine & RegionMaps[region];
					if (tempMap.IsEmpty)
					{
						continue;
					}

					int c1 = tempMap.SetAt(0), c2 = tempMap.SetAt(1);
					if (!EmptyMap[c1])
					{
						int d1 = grid[c1];
						distinctionMask ^= (short)(1 << d1);
						appearedDigitsMask |= (short)(1 << d1);
					}
					if (!EmptyMap[c2])
					{
						int d2 = grid[c2];
						distinctionMask ^= (short)(1 << d2);
						appearedDigitsMask |= (short)(1 << d2);
					}
				}
				if (!distinctionMask.IsPowerOfTwo())
				{
					continue;
				}

				short pairMask = 0;
				foreach (int cell in pair)
				{
					pairMask |= grid.GetCandidateMask(cell);
				}

				// Iterate on each combination.
				foreach (int[] digits in GetCombinationsOfArray(pairMask.GetAllSets().ToArray(), 2))
				{
					// Step 2: To determine whether the digits in pair cells
					// will only appears in square cells.
					int d1 = digits[0], d2 = digits[1];
					var appearingMap = (CandMaps[d1] | CandMaps[d2]) & square;
					if (appearingMap.Count != 4)
					{
						continue;
					}

					int block = square.BlockMask.FindFirstSet();
					var globalMap = (CandMaps[d1] | CandMaps[d2]) & RegionMaps[block];
					if (appearingMap != globalMap)
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

					// Qdp forms.
					// Now check each type.
					short comparer = (short)(1 << d1 | 1 << d2);
					CheckType1(accumulator, grid, isRow, pair, square, baseLine, pattern, pairMask, comparer);
					CheckType2(accumulator, grid, isRow, pair, square, baseLine, pattern, pairMask, comparer);
				}
			}
		}

		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isRow,
			GridMap pair, GridMap square, GridMap baseLine, Pattern pattern, short pairMask, short comparer)
		{
			short otherDigitsMask = (short)(pairMask & ~comparer);
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
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isRow,
			GridMap pair, GridMap square, GridMap baseLine, Pattern pattern, short pairMask, short comparer)
		{
			short otherDigitsMask = (short)(pairMask & ~comparer);
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
	}
}
