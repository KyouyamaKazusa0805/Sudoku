using Sudoku.Data;
using Sudoku.DocComments;
using System.Collections.Generic;
using System.Extensions;
using static Sudoku.Constants.Tables;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	partial class AlsNetStepSearcher
	{
		/// <summary>
		/// Indicates the list initialized with the static constructor.
		/// </summary>
		private static readonly IReadOnlyList<Cells> Patterns;


		/// <inheritdoc cref="StaticConstructor"/>
		static AlsNetStepSearcher()
		{
			const int a = ~7, b = ~56, c = ~448;
			int[,] sizeList = { { 3, 3 }, { 3, 4 }, { 4, 3 }, { 4, 4 }, { 4, 5 }, { 5, 4 } };
			int[] z = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
			var result = new Cells[74601];
			int n = 0;
			for (int i = 0; i < sizeList.Length >> 1; i++)
			{
				int rows = sizeList[i, 0], columns = sizeList[i, 1];
				foreach (int[] rowList in z.GetSubsets(rows))
				{
					short rowMask = 0;
					var rowMap = Cells.Empty;
					foreach (int row in rowList)
					{
						rowMask |= (short)(1 << row);
						rowMap |= RegionMaps[row + 9];
					}

					if ((rowMask & a) == 0 || (rowMask & b) == 0 || (rowMask & c) == 0)
					{
						continue;
					}

					foreach (int[] columnList in z.GetSubsets(columns))
					{
						short columnMask = 0;
						var columnMap = Cells.Empty;
						foreach (int column in columnList)
						{
							columnMask |= (short)(1 << column);
							columnMap |= RegionMaps[column + 18];
						}

						if ((columnMask & a) == 0 || (columnMask & b) == 0 || (columnMask & c) == 0)
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
