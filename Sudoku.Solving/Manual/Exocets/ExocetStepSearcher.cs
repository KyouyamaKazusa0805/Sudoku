using System;
using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// <para>
	/// Encapsulates an <b>exocet</b> technique searcher. The pattern will be like:
	/// <code>
	/// .-------.-------.-------.
	/// | B B E | E . . | E . . |
	/// | . . E | Q . . | R . . |
	/// | . . E | Q . . | R . . |
	/// :-------+-------+-------:
	/// | . . S | S . . | S . . |
	/// | . . S | S . . | S . . |
	/// | . . S | S . . | S . . |
	/// :-------+-------+-------:
	/// | . . S | S . . | S . . |
	/// | . . S | S . . | S . . |
	/// | . . S | S . . | S . . |
	/// '-------'-------'-------'
	/// </code>
	/// Where:
	/// <list type="table">
	/// <item><term>B</term><description>Base Cells.</description></item>
	/// <item><term>Q</term><description>1st Object Pair (Target cells pair 1).</description></item>
	/// <item><term>R</term><description>2nd Object Pair (Target cells pair 2).</description></item>
	/// <item><term>S</term><description>Cross-line Cells.</description></item>
	/// <item><term>E</term><description>Escape Cells.</description></item>
	/// </list>
	/// </para>
	/// <para>
	/// In the data structure, all letters will be used as the same one in this exemplar.
	/// In addition, if senior exocet, one of two target cells will lie in cross-line cells,
	/// and the lines of two target cells lying on can't contain any base digits.
	/// </para>
	/// </summary>
	public abstract class ExocetStepSearcher : StepSearcher
	{
		/// <summary>
		/// Indicates all patterns.
		/// </summary>
		protected static readonly Pattern[] Patterns = new Pattern[1458];


		/// <summary>
		/// Indicates whether the searcher will find advanced eliminations.
		/// </summary>
		protected readonly bool CheckAdvanced;


		/// <summary>
		/// Initializes an instance with the specified region maps.
		/// </summary>
		/// <param name="checkAdvanced">
		/// Indicates whether the searcher will find advanced eliminations.
		/// </param>
		protected ExocetStepSearcher(bool checkAdvanced) => CheckAdvanced = checkAdvanced;


		/// <inheritdoc cref="StaticConstructor"/>
		[SkipLocalsInit]
		static ExocetStepSearcher()
		{
			int[,] s = { { 3, 4, 5, 6, 7, 8 }, { 0, 1, 2, 6, 7, 8 }, { 0, 1, 2, 3, 4, 5 } };
			int[,] b =
			{
				{0, 1}, {0, 2}, {1, 2}, {9, 10}, {9, 11}, {10, 11}, {18, 19}, {18, 20}, {19, 20},
				{0, 9}, {0, 18}, {9, 18}, {1, 10}, {1, 19}, {10, 19}, {2, 11}, {2, 20}, {11, 20}
			};
			int[,] rq =
			{
				{9, 18}, {10, 19}, {11, 20}, {0, 18}, {1, 19}, {2, 20}, {0, 9}, {1, 10}, {2, 11},
				{1, 2}, {10, 11}, {19, 20}, {0, 2}, {9, 11}, {18, 20}, {0, 1}, {9, 10}, {18, 19}
			};
			int[,] m =
			{
				{10, 11, 19, 20}, {9, 11, 18, 20}, {9, 10, 18, 19}, {1, 2, 19, 20}, {0, 2, 18, 20},
				{0, 1, 18, 19}, {1, 2, 10, 11}, {0, 2, 9, 11}, {0, 1, 9, 10}, {10, 19, 11, 20},
				{1, 19, 2, 20}, {1, 10, 2, 11}, {9, 18, 11, 20}, {0, 18, 2, 20}, {0, 9, 2, 11},
				{9, 18, 10, 19}, {0, 18, 1, 19}, {0, 9, 1, 10}
			};
			int[] bb = { 0, 3, 6, 27, 30, 33, 54, 57, 60, 0, 27, 54, 3, 30, 57, 6, 33, 60 };
			int[,] bc =
			{
				{1, 2}, {0, 2}, {0, 1}, {4, 5}, {3, 5}, {3, 4}, {7, 8}, {6, 8}, {6, 7},
				{3, 6}, {0, 6}, {0, 3}, {4, 7}, {1, 7}, {1, 4}, {5, 8}, {2, 8}, {2, 5}
			};

			var t = (stackalloc int[3]);
			var crossline = (stackalloc int[25]); // Only use [7]..[24].
			int n = 0;
			for (int i = 0; i < 18; i++)
			{
				for (int z = i / 9 * 9, j = z; j < z + 9; j++)
				{
					for (int y = j / 3 * 3, k = y; k < y + 3; k++)
					{
						for (int l = y; l < y + 3; l++)
						{
							ref var exocet = ref Patterns[n];
							var (b1, b2) = (bb[i] + b[j, 0], bb[i] + b[j, 1]);
							var (tq1, tr1) = (bb[bc[i, 0]] + rq[k, 0], bb[bc[i, 1]] + rq[l, 0]);

							int index = 6, x = i / 3 % 3;
							int tt = i < 9 ? b1 % 9 + b2 % 9 : b1 / 9 + b2 / 9;
							tt = tt switch { < 4 => 3 - tt, < 13 => 12 - tt, _ => 21 - tt };

							(t[0], t[1], t[2]) = i < 9 ? (tt, tq1 % 9, tr1 % 9) : (tt, tq1 / 9, tr1 / 9);
							for (int index1 = 0, r = default, c = default; index1 < 3; index1++)
							{
								(i < 9 ? ref c : ref r) = t[index1];
								for (int index2 = 0; index2 < 6; index2++)
								{
									(i < 9 ? ref r : ref c) = s[x, index2];

									crossline[++index] = r * 9 + c;
								}
							}

							exocet = new(
								b1,
								b2,
								tq1,
								bb[bc[i, 0]] + rq[k, 1],
								tr1,
								bb[bc[i, 1]] + rq[l, 1],
								new(crossline[7..]),
								new() { bb[bc[i, 1]] + m[l, 2], bb[bc[i, 1]] + m[l, 3] },
								new() { bb[bc[i, 1]] + m[l, 0], bb[bc[i, 1]] + m[l, 1] },
								new() { bb[bc[i, 0]] + m[k, 2], bb[bc[i, 0]] + m[k, 3] },
								new() { bb[bc[i, 0]] + m[k, 0], bb[bc[i, 0]] + m[k, 1] });

							n++;
						}
					}
				}
			}
		}

		/// <summary>
		/// Check mirror eliminations.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="target">The target cell.</param>
		/// <param name="target2">
		/// The another target cell that is adjacent with <paramref name="target"/>.
		/// </param>
		/// <param name="lockedNonTarget">The locked member that is non-target digits.</param>
		/// <param name="baseCandidateMask">The base candidate mask.</param>
		/// <param name="mirror">(<see langword="in"/> parameter) The mirror map.</param>
		/// <param name="x">The x.</param>
		/// <param name="onlyOne">The only one cell.</param>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <param name="candidateOffsets">The candidate offsets.</param>
		/// <returns>All mirror eliminations.</returns>
		[SkipLocalsInit]
		protected (Target, Mirror) CheckMirror(
			in SudokuGrid grid, int target, int target2, short lockedNonTarget, short baseCandidateMask,
			in Cells mirror, int x, int onlyOne, IList<DrawingInfo> cellOffsets,
			IList<DrawingInfo> candidateOffsets)
		{
			var targetElims = new Target();
			var mirrorElims = new Mirror();
			int[] offsets = mirror.ToArray();
			int l = offsets[0], r = offsets[1];
			short mirrorCandsMask = (short)(grid.GetCandidates(l) | grid.GetCandidates(r));
			short commonBase = (short)(mirrorCandsMask & baseCandidateMask & grid.GetCandidates(target));
			short targetElim = (short)(grid.GetCandidates(target) & ~(short)(commonBase | lockedNonTarget));
			if (targetElim != 0)
			{
				foreach (int digit in targetElim)
				{
					targetElims.Add(new(ConclusionType.Elimination, target, digit));
				}
			}

			if (CheckAdvanced)
			{
				var regions = (stackalloc int[2]);
				short m1 = (short)(grid.GetCandidates(l) & baseCandidateMask);
				short m2 = (short)(grid.GetCandidates(r) & baseCandidateMask);
				if (m1 == 0 ^ m2 == 0)
				{
					int p = m1 == 0 ? r : l;
					short candidateMask = (short)(grid.GetCandidates(p) & ~commonBase);
					if (candidateMask != 0)
					{
						cellOffsets.Add(new(3, l));
						cellOffsets.Add(new(3, r));
						foreach (int digit in candidateMask)
						{
							mirrorElims.Add(new(ConclusionType.Elimination, p, digit));
						}
					}

					return (targetElims, mirrorElims);
				}

				short nonBase = (short)(mirrorCandsMask & ~baseCandidateMask);
				regions[0] = RegionLabel.Block.ToRegion(l);
				regions[1] =
					RegionLabel.Row.ToRegion(l) == RegionLabel.Row.ToRegion(r)
					? RegionLabel.Row.ToRegion(l)
					: RegionLabel.Column.ToRegion(l);
				short locked = default;
				foreach (short mask in Algorithms.GetMaskSubsets(nonBase))
				{
					for (int i = 0; i < 2; i++)
					{
						int count = 0;
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[regions[i]][j];
							if (p == l || p == r || p == onlyOne)
							{
								continue;
							}

							if (grid.GetCandidates(p).Overlaps(mask))
							{
								count++;
							}
						}

						if (count == mask.PopCount() - 1)
						{
							for (int j = 0; j < 9; j++)
							{
								int p = RegionCells[regions[i]][j];
								if (!grid.GetCandidates(p).Overlaps(mask)
									|| grid.GetStatus(p) != Empty || p == onlyOne)
								{
									continue;
								}

								foreach (int digit in grid.GetCandidates(p) & ~mask)
								{
									candidateOffsets.Add(new(3, p * 9 + digit));
								}

								if (p == l || p == r || !grid.GetCandidates(p).ExceptOverlaps(mask))
								{
									continue;
								}

								foreach (int digit in grid.GetCandidates(p) & ~mask)
								{
									mirrorElims.Add(new(ConclusionType.Elimination, p, digit));
								}
							}

							locked = mask;
							break;
						}
					}

					if (locked != 0)
					{
						// Here you should use '|' operator rather than '||'.
						// Operator '||' won't execute the second method if the first condition is true.
						if (g(grid, l, mirrorElims, baseCandidateMask, locked)
							| g(grid, r, mirrorElims, baseCandidateMask, locked))
						{
							cellOffsets.Add(new(3, l));
							cellOffsets.Add(new(3, r));
						}

						short mask1 = grid.GetCandidates(l), mask2 = grid.GetCandidates(r);
						bool m1Locked = mask1.Overlaps(locked), m2Locked = mask2.Overlaps(locked);
						if (locked.PopCount() == 1 && m1Locked ^ m2Locked)
						{
							short targetMask = (short)(grid.GetCandidates(target) & baseCandidateMask);
							short candidateMask = (short)(
								~(grid.GetCandidates(m1Locked ? r : l) & targetMask) & targetMask);
							if (candidateMask != 0)
							{
								foreach (int digit in candidateMask)
								{
									mirrorElims.Add(new(ConclusionType.Elimination, target, digit));
								}
							}
						}

						break;

						// Gathering.
						bool g(
							in SudokuGrid grid, int p, Mirror mirrorElims, short baseCandidateMask, short locked)
						{
							short candidateMask = (short)(grid.GetCandidates(p) & ~(baseCandidateMask | locked));
							if (candidateMask != 0)
							{
								foreach (int digit in locked)
								{
									if (grid.Exists(p, digit) is true)
									{
										candidateOffsets.Add(new(1, p * 9 + digit));
									}
								}

								foreach (int digit in candidateMask)
								{
									mirrorElims.Add(new(ConclusionType.Elimination, p, digit));
								}

								return true;
							}

							return false;
						}
					}
				}
			}

			return (targetElims, mirrorElims);
		}
	}
}
