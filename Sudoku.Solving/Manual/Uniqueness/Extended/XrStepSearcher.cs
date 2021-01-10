using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using static System.Numerics.BitOperations;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Encapsulates an <b>extended rectangle</b> technique searcher.
	/// </summary>
	public sealed partial class XrStepSearcher : UniquenessStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(11, nameof(TechniqueCode.XrType1))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			foreach (var (allCellsMap, pairs, size) in Combinations)
			{
				if ((EmptyMap & allCellsMap) != allCellsMap)
				{
					continue;
				}

				// Check each pair.
				// Ensures all pairs should contains same digits
				// and the kind of digits must be greater than 2.
				bool checkKindsFlag = true;
				foreach (var (l, r) in pairs)
				{
					short tempMask = (short)(grid.GetCandidates(l) & grid.GetCandidates(r));
					if (tempMask == 0 || tempMask.IsPowerOfTwo())
					{
						checkKindsFlag = false;
						break;
					}
				}
				if (!checkKindsFlag)
				{
					// Failed to check.
					continue;
				}

				// Check the mask of cells from two regions.
				short m1 = 0, m2 = 0;
				foreach (var (l, r) in pairs)
				{
					m1 |= grid.GetCandidates(l);
					m2 |= grid.GetCandidates(r);
				}

				short resultMask = (short)(m1 | m2);
				short normalDigits = 0, extraDigits = 0;
				foreach (int digit in resultMask)
				{
					int count = 0;
					foreach (var (l, r) in pairs)
					{
						if (((grid.GetCandidates(l) & grid.GetCandidates(r)) >> digit & 1) != 0)
						{
							// Both two cells contain same digit.
							count++;
						}
					}

					(count >= 2 ? ref normalDigits : ref extraDigits) |= (short)(1 << digit);
				}

				if (PopCount((uint)normalDigits) != size)
				{
					// The number of normal digits are not enough.
					continue;
				}

				if (PopCount((uint)resultMask) == size + 1)
				{
					// Possible type 1 or 2 found.
					// Now check extra cells.
					int extraDigit = TrailingZeroCount(extraDigits);
					var extraCellsMap = allCellsMap & CandMaps[extraDigit];
					if (extraCellsMap.IsEmpty)
					{
						continue;
					}

					if (extraCellsMap.Count == 1)
					{
						CheckType1(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit);
					}

					CheckType2(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit);
				}
				else
				{
					var extraCellsMap = Cells.Empty;
					foreach (int cell in allCellsMap)
					{
						foreach (int digit in extraDigits)
						{
							if (grid[cell, digit])
							{
								extraCellsMap.AddAnyway(cell);
								break;
							}
						}
					}

					if (!extraCellsMap.InOneRegion)
					{
						continue;
					}

					CheckType3Naked(accumulator, grid, allCellsMap, normalDigits, extraDigits, extraCellsMap);
					CheckType14(accumulator, grid, allCellsMap, normalDigits, extraCellsMap);
				}
			}
		}

		partial void CheckType1(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells allCellsMap, in Cells extraCells, short normalDigits, int extraDigit);
		partial void CheckType2(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells allCellsMap, in Cells extraCells, short normalDigits, int extraDigit);
		partial void CheckType3Naked(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells allCellsMap, short normalDigits, short extraDigits, in Cells extraCellsMap);
		partial void CheckType14(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells allCellsMap, short normalDigits, in Cells extraCellsMap);
	}
}
