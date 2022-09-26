namespace Sudoku.Solving.Logical.Prototypes;

/// <summary>
/// Provides with a <b>Unique Matrix</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Matrix Type 1</item>
/// <item>Unique Matrix Type 2</item>
/// <item>Unique Matrix Type 3</item>
/// <item>Unique Matrix Type 4</item>
/// </list>
/// </summary>
public interface IUniqueMatrixStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates the patterns.
	/// </summary>
	protected static readonly CellMap[] Patterns = new CellMap[UniqueSquareTemplatesCount];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IUniqueMatrixStepSearcher()
	{
		int[,] ChuteIterator =
		{
			{ 0, 3, 6 }, { 0, 3, 7 }, { 0, 3, 8 }, { 0, 4, 6 }, { 0, 4, 7 }, { 0, 4, 8 },
			{ 0, 5, 6 }, { 0, 5, 7 }, { 0, 5, 8 },
			{ 1, 3, 6 }, { 1, 3, 7 }, { 1, 3, 8 }, { 1, 4, 6 }, { 1, 4, 7 }, { 1, 4, 8 },
			{ 1, 5, 6 }, { 1, 5, 7 }, { 1, 5, 8 },
			{ 2, 3, 6 }, { 2, 3, 7 }, { 2, 3, 8 }, { 2, 4, 6 }, { 2, 4, 7 }, { 2, 4, 8 },
			{ 2, 5, 6 }, { 2, 5, 7 }, { 2, 5, 8 }
		};

		int length = ChuteIterator.Length / 3, n = 0;
		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < length; j++)
			{
				var a = ChuteIterator[j, 0] + i * 27;
				var b = ChuteIterator[j, 1] + i * 27;
				var c = ChuteIterator[j, 2] + i * 27;
				Patterns[n++] = CellMap.Empty
					+ a + b + c
					+ (a + 9) + (b + 9) + (c + 9)
					+ (a + 18) + (b + 18) + (c + 18);
			}
		}

		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < length; j++)
			{
				var a = ChuteIterator[j, 0] * 9;
				var b = ChuteIterator[j, 1] * 9;
				var c = ChuteIterator[j, 2] * 9;
				Patterns[n++] = CellMap.Empty
					+ (a + 3 * i) + (b + 3 * i) + (c + 3 * i)
					+ (a + 1 + 3 * i) + (b + 1 + 3 * i) + (c + 1 + 3 * i)
					+ (a + 2 + 3 * i) + (b + 2 + 3 * i) + (c + 2 + 3 * i);
			}
		}
	}
}
