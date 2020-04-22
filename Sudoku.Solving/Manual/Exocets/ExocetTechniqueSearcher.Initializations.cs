using System;
using Sudoku.Data;

namespace Sudoku.Solving.Manual.Exocets
{
	partial class ExocetTechniqueSearcher
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
	}
}
