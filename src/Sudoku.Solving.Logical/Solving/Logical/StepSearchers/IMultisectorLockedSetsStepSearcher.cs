namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Multi-sector Locked Sets</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Multi-sector Locked Sets</item>
/// </list>
/// </summary>
public interface IMultisectorLockedSetsStepSearcher : INonnegativeRankStepSearcher
{
	/// <summary>
	/// Indicates the list initialized with the static constructor.
	/// </summary>
	protected static readonly CellMap[] Patterns;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IMultisectorLockedSetsStepSearcher()
	{
		const int a = ~7, b = ~56, c = ~448;
		var sizeList = new[,] { { 3, 3 }, { 3, 4 }, { 4, 3 }, { 4, 4 }, { 4, 5 }, { 5, 4 } };
		var z = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		var result = new CellMap[MultisectorLockedSetsTemplatesCount];
		var n = 0;
		for (var i = 0; i < sizeList.Length >> 1; i++)
		{
			int rows = sizeList[i, 0], columns = sizeList[i, 1];
			foreach (var rowList in z.GetSubsets(rows))
			{
				short rowMask = 0;
				var rowMap = CellMap.Empty;
				foreach (var row in rowList)
				{
					rowMask |= (short)(1 << row);
					rowMap |= HousesMap[row + 9];
				}

				if ((rowMask & a) == 0 || (rowMask & b) == 0 || (rowMask & c) == 0)
				{
					continue;
				}

				foreach (var columnList in z.GetSubsets(columns))
				{
					short columnMask = 0;
					var columnMap = CellMap.Empty;
					foreach (var column in columnList)
					{
						columnMask |= (short)(1 << column);
						columnMap |= HousesMap[column + 18];
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
