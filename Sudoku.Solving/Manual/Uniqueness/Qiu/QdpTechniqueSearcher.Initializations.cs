using Sudoku.Data;
using Sudoku.DocComments;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	partial class QdpTechniqueSearcher
	{
		/// <summary>
		/// The base line iterator.
		/// </summary>
		private static readonly int[,] BaseLineIterator =
		{
			{ 9, 10 }, { 9, 11 }, { 10, 11 }, { 12, 13 }, { 12, 14 }, { 13, 14 },
			{ 15, 16 }, { 15, 17 }, { 16, 17 }, { 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 21, 22 }, { 21, 23 }, { 22, 23 }, { 24, 25 }, { 24, 26 }, { 25, 26 }
		};

		/// <summary>
		/// The start cells iterator (used as a <see cref="Pattern.Pair"/>).
		/// </summary>
		/// <seealso cref="Pattern.Pair"/>
		private static readonly int[,] StartCells =
		{
			{ 0, 1 }, { 0, 2 }, { 1, 2 }, { 3, 4 }, { 3, 5 }, { 4, 5 },
			{ 6, 7 }, { 6, 8 }, { 7, 8 }, { 0, 9 }, { 0, 18 }, { 9, 18 },
			{ 27, 36 }, { 27, 45 }, { 36, 45 }, { 54, 63 }, { 54, 72 }, { 63, 72 }
		};


		/// <inheritdoc cref="DefaultConstructor()"/>
		static QdpTechniqueSearcher()
		{
			for (int i = 0, n = 0, length = BaseLineIterator.Length; i < length >> 1; i++)
			{
				bool isRow = i < length >> 2;
				var baseLineMap = RegionMaps[BaseLineIterator[i, 0]] | RegionMaps[BaseLineIterator[i, 1]];
				for (int j = isRow ? 0 : 9, z = 0; z < length >> 2; j++, z++)
				{
					int c1 = StartCells[j, 0], c2 = StartCells[j, 1];
					for (int k = 0; k < 9; k++, c1 += isRow ? 9 : 1, c2 += isRow ? 9 : 1)
					{
						var pairMap = new GridMap { c1, c2 };
						if (
							baseLineMap.Overlaps(pairMap)
							|| baseLineMap.Overlaps(
								RegionMaps[GetRegion(c1, RegionLabel.Block)]
								| RegionMaps[GetRegion(c2, RegionLabel.Block)]))
						{
							continue;
						}

						var squareMap =
							baseLineMap & (
								RegionMaps[GetRegion(c1, isRow ? RegionLabel.Column : RegionLabel.Row)]
								| RegionMaps[GetRegion(c2, isRow ? RegionLabel.Column : RegionLabel.Row)]);

						Patterns[n++] = new(squareMap, baseLineMap - squareMap, pairMap);
					}
				}
			}
		}
	}
}
