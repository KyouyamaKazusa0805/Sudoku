using Sudoku.DocComments;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates an <b>exocet</b> technique searcher.
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
		public bool CheckAdvanced { get; init; }


		/// <inheritdoc cref="StaticConstructor"/>
		static unsafe ExocetStepSearcher()
		{
			int[,] s = { { 3, 4, 5, 6, 7, 8 }, { 0, 1, 2, 6, 7, 8 }, { 0, 1, 2, 3, 4, 5 } };
			int[,] b =
			{
				{ 0, 1 }, { 0, 2 }, { 1, 2 },
				{ 9, 10 }, { 9, 11 }, { 10, 11 },
				{ 18, 19 }, { 18, 20 }, { 19, 20 },
				{ 0, 9 }, { 0, 18 }, { 9, 18 },
				{ 1, 10 }, { 1, 19 }, { 10, 19 },
				{ 2, 11 }, { 2, 20 }, { 11, 20 }
			};
			int[,] rq =
			{
				{ 9, 18 }, { 10, 19 }, { 11, 20 },
				{ 0, 18 }, { 1, 19 }, { 2, 20 },
				{ 0, 9 }, { 1, 10 }, { 2, 11 },
				{ 1, 2 }, { 10, 11 }, { 19, 20 },
				{ 0, 2 }, { 9, 11 }, { 18, 20 },
				{ 0, 1 }, { 9, 10 }, { 18, 19 }
			};
			int[,] m =
			{
				{ 10, 11, 19, 20 }, { 9, 11, 18, 20 }, { 9, 10, 18, 19 },
				{ 1, 2, 19, 20 }, { 0, 2, 18, 20 }, { 0, 1, 18, 19 },
				{ 1, 2, 10, 11 }, { 0, 2, 9, 11 }, { 0, 1, 9, 10 },
				{ 10, 19, 11, 20 }, { 1, 19, 2, 20 }, { 1, 10, 2, 11 },
				{ 9, 18, 11, 20 }, { 0, 18, 2, 20 }, { 0, 9, 2, 11 },
				{ 9, 18, 10, 19 }, { 0, 18, 1, 19 }, { 0, 9, 1, 10 }
			};
			int[] bb = { 0, 3, 6, 27, 30, 33, 54, 57, 60, 0, 27, 54, 3, 30, 57, 6, 33, 60 };
			int[,] bc =
			{
				{ 1, 2 }, { 0, 2 }, { 0, 1 }, { 4, 5 }, { 3, 5 }, { 3, 4 }, { 7, 8 }, { 6, 8 }, { 6, 7 },
				{ 3, 6 }, { 0, 6 }, { 0, 3 }, { 4, 7 }, { 1, 7 }, { 1, 4 }, { 5, 8 }, { 2, 8 }, { 2, 5 }
			};

			int* t = stackalloc int[3];
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
	}
}
