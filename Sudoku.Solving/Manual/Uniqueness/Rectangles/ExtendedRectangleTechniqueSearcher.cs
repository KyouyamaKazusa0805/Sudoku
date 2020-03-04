using System.Collections.Generic;
using System.Externals;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using XrType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.ExtendedRectangleType1DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates an extended rectangle technique searcher.
	/// </summary>
	public sealed class ExtendedRectangleTechniqueSearcher : RectangleTechniqueSearcher
	{
		/// <summary>
		/// The table of regions to traverse.
		/// </summary>
		private static readonly int[,] Regions = new int[18, 2]
		{
			{ 9, 10 }, { 9, 11 }, { 10, 11 },
			{ 12, 13 }, { 12, 14 }, { 13, 14 },
			{ 15, 16 }, { 15, 17 }, { 16, 17 },
			{ 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 21, 22 }, { 21, 23 }, { 22, 23 },
			{ 24, 25 }, { 24, 26 }, { 25, 26 }
		};

		/// <summary>
		/// All combinations.
		/// </summary>
		private static readonly IReadOnlyDictionary<int, IEnumerable<long>> Combinations;


		/// <summary>
		/// The static initializer of this class.
		/// </summary>
		static ExtendedRectangleTechniqueSearcher()
		{
			var list = new Dictionary<int, IEnumerable<long>>();
			for (int i = 3; i <= 7; i++)
			{
				list.Add(i, new BitCombinationGenerator(9, i));
			}

			Combinations = list;
		}


		/// <inheritdoc/>
		public override int Priority => 46;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on each region pair.
			for (int index = 0; index < 18; index++)
			{
				// Iterate on each size.
				var (r1, r2) = (Regions[index, 0], Regions[index, 1]);
				foreach (var (size, masks) in Combinations)
				{
					// Iterate on each combination.
					foreach (short mask in masks)
					{
						var positions = mask.GetAllSets();
						var allCellsMap = GridMap.Empty;
						var pairs = new List<(int, int)>();
						foreach (int pos in positions)
						{
							int c1 = RegionUtils.GetCellOffset(r1, pos);
							int c2 = RegionUtils.GetCellOffset(r2, pos);
							allCellsMap[c1] = true;
							allCellsMap[c2] = true;
							pairs.Add((c1, c2));
						}

						// Check whether all cells are in same region.
						// If so, continue the loop immediately.
						if (size == 3 && allCellsMap.AllSetsAreInOneRegion(out _))
						{
							continue;
						}

						// Check each pair.
						// Ensures all pairs should contains same digits
						// and the kind of digits must be greater than 2.
						bool checkKindsFlag = true;
						foreach (var (l, r) in pairs)
						{
							if ((grid.GetCandidates(l) | grid.GetCandidates(r)).CountSet() > 7)
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
							m1 |= grid.GetCandidatesReversal(l);
							m2 |= grid.GetCandidatesReversal(r);
						}

						int resultMask = m1 | m2;
						if (resultMask.CountSet() == size + 1)
						{
							// Possible type 1 or 2 found.
							// Check all digits.
							var normalDigits = new List<int>();
							var extraDigits = new List<int>();
							var digits = resultMask.GetAllSets();
							foreach (int digit in digits)
							{
								int count = 0;
								foreach (var (l, r) in pairs)
								{
									if (((grid.GetCandidates(l) & grid.GetCandidates(r)) >> digit & 1) == 0)
									{
										count++;
									}
								}

								if (count >= 2)
								{
									// This candidate must be in the structure.
									normalDigits.Add(digit);
								}
								else
								{
									// This candidate must be the extra digit.
									extraDigits.Add(digit);
								}
							}

							if (normalDigits.Count != size)
							{
								// The number of normal digits are not enough.
								continue;
							}

							// Now check extra cells.
							var extraCells = new List<int>();
							foreach (int cell in allCellsMap.Offsets)
							{
								if ((grid.GetCandidates(cell) >> extraDigits[0] & 1) == 0)
								{
									extraCells.Add(cell);
								}
							}

							var extraCellsMap = new GridMap(extraCells);
							if (extraCellsMap.Offsets.All(c => grid.GetCellStatus(c) != CellStatus.Empty))
							{
								continue;
							}

							// Get all eliminations and highlight candidates.
							var conclusions = new List<Conclusion>();
							var candidateOffsets = new List<(int, int)>();
							if (extraCellsMap.Count == 1)
							{
								// Type 1.
								foreach (int cell in allCellsMap.Offsets)
								{
									if (cell == extraCells[0])
									{
										foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
										{
											if (digit == extraDigits[0])
											{
												continue;
											}

											conclusions.Add(
												new Conclusion(ConclusionType.Elimination, cell, digit));
										}
									}
									else
									{
										foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
										{
											candidateOffsets.Add((0, cell * 9 + digit));
										}
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								accumulator.Add(
									new ExtendedRectangleTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new XrType1(
											cells: allCellsMap.ToArray(),
											digits: normalDigits)));
							}
							else
							{
								// Type 2.

							}
						}
						else
						{
							// Check type 3 or 4.
							foreach (var normalDigits in
								GetAllNormalDigitCombinations(resultMask.GetAllSets().ToArray()))
							{
								// TODO: Check XR type 3 and 4.
							}
						}
					}
				}
			}
		}

		private static IEnumerable<int>[] GetAllNormalDigitCombinations(IReadOnlyList<int> digits)
		{
			System.Diagnostics.Contracts.Contract.Requires(digits.Count >= 3 && digits.Count <= 7);

			// A trick.
			// List all combinations rather than using recursion.
			return digits.Count switch
			{
				3 => new[]
				{
					new[] { digits[0], digits[1] },
					new[] { digits[0], digits[2] },
					new[] { digits[1], digits[2] }
				},
				4 => new[]
				{
					new[] { digits[0], digits[1], digits[2] },
					new[] { digits[0], digits[1], digits[3] },
					new[] { digits[0], digits[2], digits[3] },
					new[] { digits[1], digits[2], digits[3] }
				},
				5 => new[]
				{
					new[] { digits[0], digits[1], digits[2], digits[3] },
					new[] { digits[0], digits[1], digits[2], digits[4] },
					new[] { digits[0], digits[1], digits[3], digits[4] },
					new[] { digits[0], digits[2], digits[3], digits[4] },
					new[] { digits[1], digits[2], digits[3], digits[4] }
				},
				6 => new[]
				{
					new[] { digits[0], digits[1], digits[2], digits[3], digits[4] },
					new[] { digits[0], digits[1], digits[2], digits[3], digits[5] },
					new[] { digits[0], digits[1], digits[2], digits[4], digits[5] },
					new[] { digits[0], digits[1], digits[3], digits[4], digits[5] },
					new[] { digits[0], digits[2], digits[3], digits[4], digits[5] },
					new[] { digits[1], digits[2], digits[3], digits[4], digits[5] }
				},
				7 => new[]
				{
					new[] { digits[0], digits[1], digits[2], digits[3], digits[4], digits[5] },
					new[] { digits[0], digits[1], digits[2], digits[3], digits[4], digits[6] },
					new[] { digits[0], digits[1], digits[2], digits[3], digits[5], digits[6] },
					new[] { digits[0], digits[1], digits[2], digits[4], digits[5], digits[6] },
					new[] { digits[0], digits[1], digits[3], digits[4], digits[5], digits[6] },
					new[] { digits[0], digits[2], digits[3], digits[4], digits[5], digits[6] },
					new[] { digits[1], digits[2], digits[3], digits[4], digits[5], digits[6] },
				},
				_ => throw Throwing.ImpossibleCase
			};
		}
	}
}
