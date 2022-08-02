namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Multi-sector Locked Sets</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Multi-sector Locked Sets</item>
/// </list>
/// </summary>
public interface IMultisectorLockedSetsStepSearcher : IRankTheoryStepSearcher
{
	/// <summary>
	/// Indicates the list initialized with the static constructor.
	/// </summary>
	protected static readonly Cells[] Patterns;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IMultisectorLockedSetsStepSearcher()
	{
		const int a = ~7, b = ~56, c = ~448;
		int[,] sizeList = { { 3, 3 }, { 3, 4 }, { 4, 3 }, { 4, 4 }, { 4, 5 }, { 5, 4 } };
		int[] z = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		var result = new Cells[MultisectorLockedSetsTemplatesCount];
		int n = 0;
		for (int i = 0, iterationLength = sizeList.Length >> 1; i < iterationLength; i++)
		{
			int rows = sizeList[i, 0], columns = sizeList[i, 1];
			foreach (int[] rowList in z.GetSubsets(rows))
			{
				short rowMask = 0;
				var rowMap = Cells.Empty;
				foreach (int row in rowList)
				{
					rowMask |= (short)(1 << row);
					rowMap |= HouseMaps[row + 9];
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
						columnMap |= HouseMaps[column + 18];
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
