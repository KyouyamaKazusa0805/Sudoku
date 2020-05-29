using Sudoku.Data;
using System.Collections.Generic;
using static Sudoku.Constants.Processings;
using static System.Algorithms;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	partial class AlsNetTechniqueSearcher
	{
		/// <summary>
		/// Indicates the list initialized with the static constructor.
		/// </summary>
		private static readonly IReadOnlyList<GridMap> Patterns;


		/// <include file='../../../../GlobalDocComments.xml' path='comments/staticConstructor'/>
		static AlsNetTechniqueSearcher()
		{
			int[,] sizeList =
			{
				{ 3, 3 }, { 3, 4 }, { 4, 3 },
				{ 4, 4 }, { 4, 5 }, { 5, 4 }
			};
			int[] z = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
			var result = new GridMap[74601];
			int n = 0;
			for (int i = 0; i < sizeList.Length >> 1; i++)
			{
				int rows = sizeList[i, 0], columns = sizeList[i, 1];
				foreach (int[] rowList in GetCombinationsOfArray(z, rows))
				{
					short rowMask = 0;
					var rowMap = GridMap.Empty;
					foreach (int row in rowList)
					{
						rowMask |= (short)(1 << row);
						rowMap |= RegionMaps[row + 9];
					}

					if ((rowMask & ~7) == 0 || (rowMask & ~56) == 0 || (rowMask & ~448) == 0)
					{
						continue;
					}

					foreach (int[] columnList in GetCombinationsOfArray(z, columns))
					{
						short columnMask = 0;
						var columnMap = GridMap.Empty;
						foreach (int column in columnList)
						{
							columnMask |= (short)(1 << column);
							columnMap |= RegionMaps[column + 18];
						}

						if ((columnMask & ~7) == 0 || (columnMask & ~56) == 0 || (columnMask & ~448) == 0)
						{
							continue;
						}

						result[n++] = rowMap & columnMap;
					}
				}
			}

			Patterns = result;
		}
	}
}
