using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Encapsulates a <b>Qiu's deadly pattern</b> technique searcher.
	/// <code>
	/// .-------.-------.-------.
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// | P P . | . . . | . . . |
	/// :-------+-------+-------:
	/// | S S B | B B B | B B B |
	/// | S S B | B B B | B B B |
	/// | . . . | . . . | . . . |
	/// :-------+-------+-------:
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// '-------'-------'-------'
	/// </code>
	/// Where:
	/// <list type="table">
	/// <item><term>P</term><description>Pair Cells.</description></item>
	/// <item><term>S</term><description>Square Cells.</description></item>
	/// <item><term>B</term><description>Base Line Cells.</description></item>
	/// </list>
	/// </summary>
	public sealed partial class QdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		private static readonly Pattern[] Patterns = new Pattern[972];


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(58, nameof(TechniqueCode.QdpType1)) { DisplayLevel = 2 };


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			for (int i = 0, length = Patterns.Length; i < length; i++)
			{
				var pattern = Patterns[i];
				bool isRow = i < length >> 1;
				var (pair, square, baseLine) = pattern;

				// To check whether both two pair cells are empty.
				if (!EmptyMap[pair.First] || !EmptyMap[pair.SetAt(1)])
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
						f(grid, tempMap, ref appearedDigitsMask, ref distinctionMask, ref appearedParts);
					}
					else if ((square & regionMap) is { IsNotEmpty: true } squareMap)
					{
						// Don't forget to record the square cells.
						f(grid, squareMap, ref appearedDigitsMask, ref distinctionMask, ref appearedParts);
					}

					static void f(
						in SudokuGrid grid, in GridMap map, ref short appearedDigitsMask,
						ref short distinctionMask, ref int appearedParts)
					{
						bool flag = false;
						int c1 = map.First, c2 = map.SetAt(1);
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

				if (!distinctionMask.IsPowerOfTwo() || appearedParts != appearedDigitsMask.PopCount())
				{
					continue;
				}

				short pairMask = 0;
				foreach (int cell in pair)
				{
					pairMask |= grid.GetCandidateMask(cell);
				}

				// Iterate on each combination.
				for (int size = 2, count = pairMask.PopCount(); size < count; size++)
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

		partial void CheckType1(IList<TechniqueInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair, in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer, short otherDigitsMask);
		partial void CheckType2(IList<TechniqueInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair, in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer, short otherDigitsMask);
		partial void CheckType3(IList<TechniqueInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair, in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer, short otherDigitsMask);
		partial void CheckType4(IList<TechniqueInfo> accumulator, bool isRow, in GridMap pair, in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer);
		partial void CheckLockedType(IList<TechniqueInfo> accumulator, in SudokuGrid grid, bool isRow, in GridMap pair, in GridMap square, in GridMap baseLine, in Pattern pattern, short comparer);
	}
}
