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
		/// The fit type XRs table (row direction).
		/// </summary>
		private static readonly int[,] FitTableRow =
		{
			{ 0, 3 }, { 0, 4 }, { 0, 5 }, { 0, 6 }, { 0, 7 }, { 0, 8 },
			{ 1, 3 }, { 1, 4 }, { 1, 5 }, { 1, 6 }, { 1, 7 }, { 1, 8 },
			{ 2, 3 }, { 2, 4 }, { 2, 5 }, { 2, 6 }, { 2, 7 }, { 2, 8 },
			{ 3, 6 }, { 3, 7 }, { 3, 8 },
			{ 4, 6 }, { 4, 7 }, { 4, 8 },
			{ 5, 6 }, { 5, 7 }, { 5, 8 },
		};

		/// <summary>
		/// The fit type XRs table (column direction).
		/// </summary>
		private static readonly int[,] FitTableColumn =
		{
			{ 0, 27 }, { 0, 36 }, { 0, 45 }, { 0, 54 }, { 0, 63 }, { 0, 72 },
			{ 9, 27 }, { 9, 36 }, { 9, 45 }, { 9, 54 }, { 9, 63 }, { 9, 72 },
			{ 18, 27 }, { 18, 36 }, { 18, 45 }, { 18, 54 }, { 18, 63 }, { 18, 72 },
			{ 27, 54 }, { 27, 63 }, { 27, 72 },
			{ 36, 54 }, { 36, 63 }, { 36, 72 },
			{ 45, 54 }, { 45, 63 }, { 45, 72 },
		};


		/// <summary>
		/// All combinations.
		/// </summary>
		private static readonly IReadOnlyList<(GridMap, IReadOnlyList<(int, int)>, int)> Combinations;


		/// <include file='../../../../GlobalDocComments.xml' path='comments/staticConstructor'/>
		static XrTechniqueSearcher()
		{
			var combinations = new List<(GridMap, IReadOnlyList<(int, int)>, int)>();

			// Fit type. e.g.
			// ab | ab
			// bc | bc
			// ac | ac
			for (int j = 0; j < 3; j++)
			{
				for (int i = 0, length = FitTableRow.Length >> 1; i < length; i++)
				{
					int c11 = FitTableRow[i, 0] + j * 27, c21 = FitTableRow[i, 1] + j * 27;
					int c12 = c11 + 9, c22 = c21 + 9;
					int c13 = c11 + 18, c23 = c21 + 18;
					combinations.Add((
						new GridMap { c11, c12, c13, c21, c22, c23 },
						new[] { (c11, c21), (c12, c22), (c13, c23) },
						3));
				}
			}
			for (int j = 0; j < 3; j++)
			{
				for (int i = 0, length = FitTableColumn.Length >> 1; i < length; i++)
				{
					int c11 = FitTableColumn[i, 0] + j * 3, c21 = FitTableColumn[i, 1] + j * 3;
					int c12 = c11 + 1, c22 = c21 + 1;
					int c13 = c11 + 2, c23 = c21 + 2;
					combinations.Add((
						new GridMap { c11, c12, c13, c21, c22, c23 },
						new[] { (c11, c21), (c12, c22), (c13, c23) },
						3));
				}
			}
			
			// Fat type. e.g.
			// ab | . ac . | bc
			// ab | . ac . | bc
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
