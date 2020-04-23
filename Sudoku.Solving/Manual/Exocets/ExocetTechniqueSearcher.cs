using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
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
	public sealed partial class ExocetTechniqueSearcher : TechniqueSearcher
	{
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
			var mapPlayground = (Span<GridMap>)stackalloc GridMap[2];
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
				short baseCandidates = (short)(m1 | m2);
				if (baseCandidates.CountSet() > 5)
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
				if ((baseCandidates & crosslineMask) != 0)
				{
					continue;
				}

				short digitsNeedChecking = (short)((short)((short)((short)(
					grid.GetCandidatesReversal(tq1) | grid.GetCandidatesReversal(tq2))
					| grid.GetCandidatesReversal(tr1)) | grid.GetCandidatesReversal(tr2)) & baseCandidates);
				int emptyCount = 0;
				(targetCells[0], targetCells[1], targetCells[2], targetCells[3]) = (tq1, tq2, tr1, tr2);

				// Target cells which is non-empty cannot hold any digits that base cell holds.
				short targetValueMask = 0;
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
							targetValueMask |= (short)(1 << grid[cell]);
							break;
						}
					}
				}
				if ((baseCandidates & targetValueMask) != 0)
				{
					continue;
				}

				switch (emptyCount)
				{
					case 1:
					{
						// TODO: Process as a senior exocet.
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
								if ((baseCandidates >> digit & 1) == 0)
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
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

						var mirrorEliminations = CheckMirror(grid, baseCandidates, in exocet, -1, cellOffsets, candidateOffsets);
						if (conclusions.Count == 0 && mirrorEliminations.Count == 0)
						{
							continue;
						}

						accumulator.Add(
							new JuniorExocetTechniqueInfo(
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
								digits: baseCandidates.GetAllSets(),
								mirrorEliminations));

						break;
					}
					case 3:
					{
						// With a conjugate pair.
						var map = new GridMap(targetCells);
						int z = default;
						foreach (int cell in targetCells)
						{
							if (grid.GetCellStatus(cell) != Empty)
							{
								z = cell;
								break;
							}
						}
						map.Remove(z);

						// Record all digits whose cell lying on is empty.
						short targetMask = 0;
						foreach (int cell in map.Offsets)
						{
							targetMask |= grid.GetCandidatesReversal(cell);
						}

						int diagonalCell = GetDiagonalCell(targetCells, z);
						map.Remove(diagonalCell);
						mapPlayground[0] = new GridMap(stackalloc[] { diagonalCell, map.SetAt(0) });
						mapPlayground[1] = new GridMap(stackalloc[] { diagonalCell, map.SetAt(1) });

						// Get all digits to iterate.
						short targetMaskWithoutValueCell = (short)(targetMask & ~(1 << grid[z]));
						foreach (var tempMap in mapPlayground)
						{
							foreach (int digit in targetMaskWithoutValueCell.GetAllSets())
							{
								var comparer = grid.GetDigitAppearingCells(digit, tempMap.CoveredLine);
								if (comparer != tempMap)
								{
									continue;
								}

								// Check eliminations.
								var conclusions = new List<Conclusion>();
								foreach (int cell in comparer.Offsets)
								{
									foreach (int d in (targetMaskWithoutValueCell & ~(1 << digit)).GetAllSets())
									{
										if ((baseCandidates >> d & 1) != 0 || !(grid.Exists(cell, d) is true))
										{
											continue;
										}

										conclusions.Add(new Conclusion(Elimination, cell, d));
									}
								}
								int anotherTargetCell = (map - comparer).SetAt(0);
								foreach (int d in grid.GetCandidatesReversal(anotherTargetCell).GetAllSets())
								{
									if ((baseCandidates >> d & 1) != 0)
									{
										continue;
									}

									conclusions.Add(new Conclusion(Elimination, anotherTargetCell, d));
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
								foreach (int d in grid.GetCandidatesReversal(b1).GetAllSets())
								{
									candidateOffsets.Add((0, b1 * 9 + d));
								}
								foreach (int d in grid.GetCandidatesReversal(b2).GetAllSets())
								{
									candidateOffsets.Add((0, b2 * 9 + d));
								}
								foreach (int cell in comparer.Offsets)
								{
									candidateOffsets.Add((1, cell * 9 + digit));
								}

								var mirrorEliminations =
									CheckMirror(grid, baseCandidates, in exocet, digit, cellOffsets, candidateOffsets);
								if (conclusions.Count == 0 && mirrorEliminations.Count == 0)
								{
									continue;
								}

								accumulator.Add(
									new JuniorExocetTechniqueInfo(
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
										digits: baseCandidates.GetAllSets(),
										mirrorEliminations));
							}
						}

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
		/// Check mirror cells.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="baseCandidates">The base candidates mask.</param>
		/// <param name="exocet">(<see langword="in"/> parameter) The exocet.</param>
		/// <param name="conjugatePairDigit">The digit of conjugate pair is available.</param>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <param name="candidateOffsets">The candidate offsets.</param>
		private MirrorEliminations CheckMirror(
			IReadOnlyGrid grid, short baseCandidates, in Exocet exocet, int conjugatePairDigit,
			IList<(int, int)> cellOffsets, IList<(int, int)> candidateOffsets)
		{
			var (_, target, _) = exocet;
			var (_, _, tq1, tq2, tr1, tr2, _, mq1, mq2, mr1, mr2) = exocet;
			return MirrorEliminations.MergeAll(p(tq1, mq1), p(tq2, mq2), p(tr1, mr1), p(tr2, mr2));

			MirrorEliminations p(int t, GridMap m)
			{
				var result = new MirrorEliminations();
				if (grid.GetCellStatus(t) != Empty)
				{
					return result;
				}

				short targetMask = grid.GetCandidatesReversal(t);
				short commonBase = targetMask;
				short mirrorCandidates = 0;
				int valueCount = 0;
				int z = default;
				foreach (int cell in m.Offsets)
				{
					mirrorCandidates |= grid.GetCandidatesReversal(cell);
					if (grid.GetCellStatus(cell) != Empty)
					{
						z = cell;
						valueCount++;
					}
				}

				switch (valueCount)
				{
					default:
					case 1 when (baseCandidates >> grid[z] & 1) != 0:
					case 2:
					{
						return result;
					}
					case 0:
					{
						// Search conjugate pair.
						short otherCandidates = (short)(mirrorCandidates & ~baseCandidates);
						foreach (int digit in otherCandidates.GetAllSets())
						{
							if (digit == conjugatePairDigit)
							{
								continue;
							}

							int region = m.CoveredLine;
							var elimMap = grid.GetDigitAppearingCells(digit, region) - target;
							if (elimMap != m)
							{
								continue;
							}

							foreach (int d in (otherCandidates & ~(1 << digit)).GetAllSets())
							{
								if (d == conjugatePairDigit)
								{
									continue;
								}

								foreach (int cell in elimMap.Offsets)
								{
									if (grid.Exists(cell, d) is true)
									{
										result.Add(new Conclusion(Elimination, cell, d));
									}
								}
							}
							foreach (int cell in m.Offsets)
							{
								candidateOffsets.Add((1, cell * 9 + digit));
								cellOffsets.Add((3, cell));
							}
						}

						return result;
					}
					case 1:
					{
						commonBase &= mirrorCandidates;
						commonBase &= baseCandidates;
						if (commonBase == 0)
						{
							return result;
						}

						foreach (int digit in ((short)(511 & ~commonBase)).GetAllSets())
						{
							foreach (int cell in m.Offsets)
							{
								if (grid.Exists(cell, digit) is true)
								{
									result.Add(new Conclusion(Elimination, cell, digit));
								}
							}
						}
						foreach (int digit in targetMask.GetAllSets())
						{
							if ((commonBase >> digit & 1) == 0)
							{
								result.Add(new Conclusion(Elimination, t, digit));
							}
						}

						foreach (int cell in m.Offsets)
						{
							cellOffsets.Add((3, cell));
						}

						return result;
					}
				}
			}
		}


		/// <summary>
		/// Get the diagonal cell in target cells.
		/// </summary>
		/// <param name="cells">All target cells.</param>
		/// <param name="cell">The cell.</param>
		/// <returns>The diagonal cell.</returns>
		private static int GetDiagonalCell(ReadOnlySpan<int> cells, int cell)
		{
			return true switch
			{
				_ when cells[0] == cell => cells[3],
				_ when cells[1] == cell => cells[2],
				_ when cells[2] == cell => cells[1],
				_ => cells[0]
			};
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
