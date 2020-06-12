using Sudoku.Data;
using Sudoku.Extensions;
using System.Collections.Generic;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	partial class XrTechniqueSearcher
	{
		/// <summary>
		/// The table of regions to traverse.
		/// </summary>
		private static readonly int[,] Regions =
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
		private static readonly IReadOnlyList<(GridMap, IReadOnlyList<(int, int)>, int)> Combinations;


		/// <include file='../../../../GlobalDocComments.xml' path='comments/staticConstructor'/>
		static XrTechniqueSearcher()
		{
			var combinations = new List<(GridMap, IReadOnlyList<(int, int)>, int)>();
			for (int size = 3; size <= 7; size++)
			{
				for (int i = 0, length = Regions.Length >> 1; i < length; i++)
				{
					int region1 = Regions[i, 0], region2 = Regions[i, 1];
					foreach (short mask in new BitCombinationGenerator(9, size))
					{
						// Check whether all cells are in same region.
						// If so, continue the loop immediately.
						short a = (short)(mask >> 6), b = (short)(mask >> 3 & 7), c = (short)(mask & 7);
						if (size == 3 && (a == 7 || b == 7 || c == 7))
						{
							continue;
						}

						var map = GridMap.Empty;
						var pairs = new List<(int, int)>();
						foreach (int pos in mask.GetAllSets())
						{
							int cell1 = RegionCells[region1][pos], cell2 = RegionCells[region2][pos];
							map.Add(cell1);
							map.Add(cell2);
							pairs.Add((cell1, cell2));
						}

						combinations.Add((map, pairs, size));
					}
				}
			}

			Combinations = combinations;
		}
	}
}
