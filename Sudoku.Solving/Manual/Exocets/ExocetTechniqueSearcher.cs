using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Data.CellStatus;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// <para>Encapsulates an <b>exocet</b> technique searcher.</para>
	/// <para>
	/// The pattern will be like:
	/// <code>
	/// .-------.-------.-------.<br/>
	/// | B B * | * . . | * . . | B = Base Cells<br/>
	/// | . . * | Q . . | R . . | Q = 1st Object Pair<br/>
	/// | . . * | Q . . | R . . | R = 2nd Object Pair<br/>
	/// :-------+-------+-------: S = Cross-line Cells<br/>
	/// | . . S | S . . | S . . | * = Escape Cells<br/>
	/// | . . S | S . . | S . . |<br/>
	/// | . . S | S . . | S . . |<br/>
	/// :-------+-------+-------:<br/>
	/// | . . S | S . . | S . . |<br/>
	/// | . . S | S . . | S . . |<br/>
	/// | . . S | S . . | S . . |<br/>
	/// '-------'-------'-------'
	/// </code>
	/// </para>
	/// </summary>
	public sealed class ExocetTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// The cross line cells iterator.
		/// </summary>
		private static readonly int[,] SIterator =
		{
			{ 3, 4, 5, 6, 7, 8 }, { 0, 1, 2, 6, 7, 8 }, { 0, 1, 2, 3, 4, 5 }
		};

		/// <summary>
		/// The base cells iterator.
		/// </summary>
		private static readonly int[,] BIterator =
		{
			{0, 1}, {0, 2}, {1, 2}, {9, 10}, {9, 11}, {10, 11}, {18, 19}, {18, 20}, {19, 20},
			{0, 9}, {0, 18}, {9, 18}, {1, 10}, {1, 19}, {10, 19}, {2, 11}, {2, 20}, {11, 20}
		};

		/// <summary>
		/// The Q or R cells iterator.
		/// </summary>
		private static readonly int[,] RqIterator =
		{
			{9, 18}, {10, 19}, {11, 20}, {0, 18}, {1, 19}, {2, 20}, {0, 9}, {1, 10}, {2, 11},
			{1, 2}, {10, 11}, {19, 20}, {0, 2}, {9, 11}, {18, 20}, {0, 1}, {9, 10}, {18, 19}
		};

		/// <summary>
		/// The mirror list.
		/// </summary>
		private static readonly int[,] Mirror =
		{
			{10, 11, 19, 20}, {9, 11, 18, 20}, {9, 10, 18, 19}, {1, 2, 19, 20}, {0, 2, 18, 20},
			{0, 1, 18, 19}, {1, 2, 10, 11}, {0, 2, 9, 11}, {0, 1, 9, 10}, {10, 19, 11, 20},
			{1, 19, 2, 20}, {1, 10, 2, 11}, {9, 18, 11, 20}, {0, 18, 2, 20}, {0, 9, 2, 11},
			{9, 18, 10, 19}, {0, 18, 1, 19}, {0, 9, 1, 10}
		};

		private static readonly int[] B_s =
		{
			0, 3, 6, 27, 30, 33, 54, 57, 60, 0, 27, 54, 3, 30, 57, 6, 33, 60
		};

		private static readonly int[,] B_rq =
		{
			{1, 2}, {0, 2}, {0, 1}, {4, 5}, {3, 5}, {3, 4}, {7, 8}, {6, 8}, {6, 7},
			{3, 6}, {0, 6}, {0, 3}, {4, 7}, {1, 7}, {1, 4}, {5, 8}, {2, 8}, {2, 5}
		};

		/// <summary>
		/// Indicates all exocet patterns to iterate on.
		/// </summary>
		private static readonly Exocet[] Exocets;


		/// <summary>
		/// Indicates the region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the specified region maps.
		/// </summary>
		/// <param name="regionMaps">The region maps.</param>
		public ExocetTechniqueSearcher(GridMap[] regionMaps) => _regionMaps = regionMaps;


		/// <summary>
		/// The static constructor of <see cref="ExocetTechniqueSearcher"/>.
		/// </summary>
		static ExocetTechniqueSearcher()
		{
			var t = (Span<int>)stackalloc int[3];
			var tt = (Span<int>)stackalloc int[25]; // Only use [7]..[24].
			int n = 0;
			Exocets = new Exocet[1458];
			for (int i = 0; i < 18; i++)
			{
				for (int z = i / 9 * 9, j = z; j < z + 9; j++)
				{
					for (int y = j / 3 * 3, k = y; k < y + 3; k++)
					{
						for (int l = y; l < y + 3; l++)
						{
							ref var exocet = ref Exocets[n];
							var (b1, b2) = (B_s[i] + BIterator[j, 0], B_s[i] + BIterator[j, 1]);
							var (tq1, tr1) = (
								B_s[B_rq[i, 0]] + RqIterator[k, 0],
								B_s[B_rq[i, 1]] + RqIterator[l, 0]);

							int ll = 6, x = i / 3 % 3;
							int m = i < 9 ? b1 % 9 + b2 % 9 : b1 / 9 + b2 / 9;
							m = true switch
							{
								_ when m < 4 => 3 - m,
								_ when m < 13 => 12 - m,
								_ => 21 - m
							};
							(t[0], t[1], t[2]) = i < 9 ? (m, tq1 % 9, tr1 % 9) : (m, tq1 / 9, tr1 / 9);

							for (int a = 0, r = default, c = default; a < 3; a++)
							{
								(i < 9 ? ref c : ref r) = t[a];
								for (int b = 0; b < 6; b++)
								{
									(i < 9 ? ref r : ref c) = SIterator[x, b];

									tt[++ll] = r * 9 + c;
								}
							}

							exocet = new Exocet(
								b1,
								b2,
								tq1,
								B_s[B_rq[i, 0]] + RqIterator[k, 1],
								tr1,
								B_s[B_rq[i, 1]] + RqIterator[l, 1],
								new GridMap(tt[7..]),
								new GridMap(stackalloc[] { B_s[B_rq[i, 1]] + Mirror[l, 2], B_s[B_rq[i, 1]] + Mirror[l, 3] }),
								new GridMap(stackalloc[] { B_s[B_rq[i, 1]] + Mirror[l, 0], B_s[B_rq[i, 1]] + Mirror[l, 1] }),
								new GridMap(stackalloc[] { B_s[B_rq[i, 0]] + Mirror[k, 2], B_s[B_rq[i, 0]] + Mirror[k, 3] }),
								new GridMap(stackalloc[] { B_s[B_rq[i, 0]] + Mirror[k, 0], B_s[B_rq[i, 0]] + Mirror[k, 1] }));

							n++;
						}
					}
				}
			}
		}


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 94;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var targetCells = (Span<int>)stackalloc int[4];
			var digitDistributions = GetDigitDistributions(grid);
			foreach (var exocet in Exocets)
			{
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;

				// The base cells cannot be given or modifiable.
				if (grid.GetCellStatus(b1) != Empty || grid.GetCellStatus(b2) != Empty)
				{
					continue;
				}

				// The number of different candidates in base cells cannot be greater than 5.
				short m1 = grid.GetCandidatesReversal(b1);
				short m2 = grid.GetCandidatesReversal(b2);
				short m = (short)(m1 | m2);
				if (m.CountSet() > 5)
				{
					continue;
				}

				// Any cells in the cross line region cannot contain the digit that
				// base cells hold.
				short crosslineMask = 0;
				foreach (int cell in s.Offsets)
				{
					if (grid.GetCellStatus(cell) != Empty)
					{
						crosslineMask |= (short)(1 << grid[cell]);
					}
				}
				if ((m & crosslineMask) != 0)
				{
					continue;
				}

				short digitsNeedChecking = (short)((short)((short)((short)(
					grid.GetCandidatesReversal(tq1) | grid.GetCandidatesReversal(tq2))
					| grid.GetCandidatesReversal(tr1)) | grid.GetCandidatesReversal(tr2)) & m);
				int emptyCount = 0;
				(targetCells[0], targetCells[1], targetCells[2], targetCells[3]) = (tq1, tq2, tr1, tr2);

				// Target cells which is non-empty cannot hold any digits that base cell holds.
				short targetMask = 0;
				foreach (int cell in targetCells)
				{
					switch (grid.GetCellStatus(cell))
					{
						case Empty:
						{
							emptyCount++;
							break;
						}
						case Modifiable:
						case Given:
						{
							targetMask |= (short)(1 << grid[cell]);
							break;
						}
					}
				}
				if ((m & targetMask) != 0)
				{
					continue;
				}

				switch (emptyCount)
				{
					case 0:
					{
						continue;
					}
					case 1:
					{
						// May not consider now.
						break;
					}
					case 2:
					{
						int region = new GridMap(stackalloc[] { b1, b2 }).CoveredLine;
						bool isRow = region >= 9 && region <= 18;
						if (!CheckCrossLine(s, isRow, digitsNeedChecking, digitDistributions))
						{
							continue;
						}

						// Now check eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int cell in targetCells)
						{
							if (grid.GetCellStatus(cell) != Empty)
							{
								continue;
							}

							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								if ((m >> digit & 1) == 0)
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var cellOffsets = new List<(int, int)>
						{
							(0, b1), (0, b2), (1, tq1), (1, tq2), (1, tr1), (1, tr2)
						};
						foreach (int cell in s.Offsets)
						{
							cellOffsets.Add((2, cell));
						}
						var candidateOffsets = new List<(int, int)>();
						foreach (int digit in grid.GetCandidatesReversal(b1).GetAllSets())
						{
							candidateOffsets.Add((0, b1 * 9 + digit));
						}
						foreach (int digit in grid.GetCandidatesReversal(b2).GetAllSets())
						{
							candidateOffsets.Add((0, b2 * 9 + digit));
						}

						accumulator.Add(
							new ExocetTechniqueInfo(
								conclusions,
								views: new[]
								{
										new View(
											cellOffsets,
											candidateOffsets,
											regionOffsets: null,
											links: null)
								},
								exocet,
								digits: m.GetAllSets(),
								isSenior: false));

						break;
					}
					case 3:
					{
						// TODO: With a strong link.
						break;
					}
					case 4:
					{
						// TODO: With two strong links.
						break;
					}
				}
			}
		}

		/// <summary>
		/// Check the cross line cells.
		/// </summary>
		/// <param name="crossline">The cross line cells.</param>
		/// <param name="isRow">Indicates whether the specified checking is for rows.</param>
		/// <param name="digitsNeedChecking">The digits that need checking.</param>
		/// <param name="digitDistributions">All digit distributions.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private bool CheckCrossLine(
			GridMap crossline, bool isRow, short digitsNeedChecking, GridMap[] digitDistributions)
		{
			foreach (int digit in digitsNeedChecking.GetAllSets())
			{
				var s = crossline & digitDistributions[digit];
				if ((isRow ? s.RowMask : s.ColumnMask).CountSet() > 2)
				{
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// Get all distributions for digits.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The digit distributions.</returns>
		private static GridMap[] GetDigitDistributions(IReadOnlyGrid grid)
		{
			var digitDistributions = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref digitDistributions[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if ((grid.GetCandidates(cell) >> digit & 1) == 0)
					{
						map.Add(cell);
					}
				}
			}

			return digitDistributions;
		}

		/// <summary>
		/// Get all combinations that contains all set bits from the specified number.
		/// </summary>
		/// <param name="seed">The specified number.</param>
		/// <returns>All combinations.</returns>
		private static IEnumerable<short> GetCombinations(short seed)
		{
			for (int i = 0; i < 9; i++)
			{
				foreach (short mask in new BitCombinationGenerator(9, i))
				{
					if ((mask & seed) == mask)
					{
						yield return mask;
					}
				}
			}
		}
	}
}
