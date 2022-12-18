namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Qiu's Deadly Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Qiu's Deadly Pattern Type 1</item>
/// <item>Qiu's Deadly Pattern Type 2</item>
/// <item>Qiu's Deadly Pattern Type 3</item>
/// <item>Qiu's Deadly Pattern Type 4</item>
/// <item>Qiu's Deadly Pattern Locked Type</item>
/// </list>
/// </summary>
public interface IQiuDeadlyPatternStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// All different patterns.
	/// </summary>
	protected static readonly QiuDeadlyPattern[] Patterns = new QiuDeadlyPattern[QiuDeadlyPatternTemplatesCount];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IQiuDeadlyPatternStepSearcher()
	{
		int[,] BaseLineIterator =
		{
			{ 9, 10 }, { 9, 11 }, { 10, 11 }, { 12, 13 }, { 12, 14 }, { 13, 14 },
			{ 15, 16 }, { 15, 17 }, { 16, 17 }, { 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 21, 22 }, { 21, 23 }, { 22, 23 }, { 24, 25 }, { 24, 26 }, { 25, 26 }
		};
		int[,] StartCells =
		{
			{ 0, 1 }, { 0, 2 }, { 1, 2 }, { 3, 4 }, { 3, 5 }, { 4, 5 },
			{ 6, 7 }, { 6, 8 }, { 7, 8 }, { 0, 9 }, { 0, 18 }, { 9, 18 },
			{ 27, 36 }, { 27, 45 }, { 36, 45 }, { 54, 63 }, { 54, 72 }, { 63, 72 }
		};

		for (int i = 0, n = 0, length = BaseLineIterator.Length; i < length >> 1; i++)
		{
			var isRow = i < length >> 2;
			var baseLineMap = HousesMap[BaseLineIterator[i, 0]] | HousesMap[BaseLineIterator[i, 1]];
			for (int j = isRow ? 0 : 9, z = 0; z < length >> 2; j++, z++)
			{
				int c1 = StartCells[j, 0], c2 = StartCells[j, 1];
				for (var k = 0; k < 9; k++, c1 += isRow ? 9 : 1, c2 += isRow ? 9 : 1)
				{
					var pairMap = CellsMap[c1] + c2;
					if (baseLineMap && pairMap)
					{
						continue;
					}

					var tempMapBlock = HousesMap[c1.ToHouseIndex(HouseType.Block)] | HousesMap[c2.ToHouseIndex(HouseType.Block)];
					if (baseLineMap && tempMapBlock)
					{
						continue;
					}

					var tempMapLine = HousesMap[c1.ToHouseIndex(isRow ? HouseType.Column : HouseType.Row)]
						| HousesMap[c2.ToHouseIndex(isRow ? HouseType.Column : HouseType.Row)];
					var squareMap = baseLineMap & tempMapLine;
					Patterns[n++] = new(squareMap, baseLineMap - squareMap, pairMap);
				}
			}
		}
	}
}
