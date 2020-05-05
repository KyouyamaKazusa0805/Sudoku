using System;
using Sudoku.Data;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	partial class SkLoopTechniqueSearcher
	{
		/// <summary>
		/// The position table of all SK-loops.
		/// </summary>
		private static readonly int[][] SkLoopTable = new int[729][];

		/// <summary>
		/// The region maps.
		/// </summary>
		private static readonly GridMap[] RegionMaps;


		/// <include file='../../../../GlobalDocComments.xml' path='comments/staticConstructor'/>
		static SkLoopTechniqueSearcher()
		{
			// Initialize for region maps.
			RegionMaps = new GridMap[27];
			for (int i = 0; i < 27; i++)
			{
				ref var map = ref RegionMaps[i];
				foreach (int cell in RegionCells[i])
				{
					map.Add(cell);
				}
			}

			// Initialize for SK-loop table.
			var s = (Span<int>)stackalloc int[4];
			for (int a = 9, n = 0; a < 18; a++)
			{
				for (int b = 9; b < 18; b++)
				{
					if (a / 3 == b / 3 || b < a) continue;

					for (int c = 18; c < 27; c++)
					{
						for (int d = 18; d < 27; d++)
						{
							if (c / 3 == d / 3 || d < c) continue;

							var all = RegionMaps[a] | RegionMaps[b] | RegionMaps[c] | RegionMaps[d];
							var overlap = (RegionMaps[a] | RegionMaps[b]) & (RegionMaps[c] | RegionMaps[d]);
							short blockMask = overlap.BlockMask;
							for (int i = 0, count = 0; count < 4 && i < 16; i++)
							{
								if ((blockMask >> i & 1) != 0)
								{
									s[count++] = i;
								}
							}

							all &= RegionMaps[s[0]] | RegionMaps[s[1]] | RegionMaps[s[2]] | RegionMaps[s[3]];
							all -= overlap;

							SkLoopTable[n] = new int[16];
							int pos = 0;
							foreach (int cell in (all & RegionMaps[a]).Offsets)
							{
								SkLoopTable[n][pos++] = cell;
							}
							foreach (int cell in (all & RegionMaps[d]).Offsets)
							{
								SkLoopTable[n][pos++] = cell;
							}
							int[] cells = (all & RegionMaps[b]).ToArray();
							SkLoopTable[n][pos++] = cells[2];
							SkLoopTable[n][pos++] = cells[3];
							SkLoopTable[n][pos++] = cells[0];
							SkLoopTable[n][pos++] = cells[1];
							cells = (all & RegionMaps[c]).ToArray();
							SkLoopTable[n][pos++] = cells[2];
							SkLoopTable[n][pos++] = cells[3];
							SkLoopTable[n][pos++] = cells[0];
							SkLoopTable[n++][pos++] = cells[1];
						}
					}
				}
			}
		}
	}
}
