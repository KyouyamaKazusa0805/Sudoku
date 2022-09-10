namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Provides with an <b>Extended Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Rectangle Type 1</item>
/// <item>Extended Rectangle Type 2</item>
/// <item>Extended Rectangle Type 3</item>
/// <item>Extended Rectangle Type 4</item>
/// </list>
/// </summary>
public interface IExtendedRectangleStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates all possible extended rectangle pattern combinations.
	/// </summary>
	/// <remarks>
	/// <para>The list contains two types of <b>Extended Rectangle</b>s:</para>
	/// <para>
	/// Fit type (2 blocks spanned):
	/// <code><![CDATA[
	/// ab | ab
	/// bc | bc
	/// ac | ac
	/// ]]></code>
	/// </para>
	/// <para>
	/// Fat type (3 blocks spanned):
	/// <code><![CDATA[
	/// ab | ac | bc
	/// ab | ac | bc
	/// ]]></code>
	/// </para>
	/// </remarks>
	protected static readonly IReadOnlyList<(CellMap Cells, IReadOnlyList<(int Left, int Right)> PairCells, int Size)> PatternInfos;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IExtendedRectangleStepSearcher()
	{
		int[,] houses =
		{
			{ 9, 10 }, { 9, 11 }, { 10, 11 },
			{ 12, 13 }, { 12, 14 }, { 13, 14 },
			{ 15, 16 }, { 15, 17 }, { 16, 17 },
			{ 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 21, 22 }, { 21, 23 }, { 22, 23 },
			{ 24, 25 }, { 24, 26 }, { 25, 26 }
		},
		fitTableRow =
		{
			{ 0, 3 }, { 0, 4 }, { 0, 5 }, { 0, 6 }, { 0, 7 }, { 0, 8 },
			{ 1, 3 }, { 1, 4 }, { 1, 5 }, { 1, 6 }, { 1, 7 }, { 1, 8 },
			{ 2, 3 }, { 2, 4 }, { 2, 5 }, { 2, 6 }, { 2, 7 }, { 2, 8 },
			{ 3, 6 }, { 3, 7 }, { 3, 8 },
			{ 4, 6 }, { 4, 7 }, { 4, 8 },
			{ 5, 6 }, { 5, 7 }, { 5, 8 }
		},
		fitTableColumn =
		{
			{ 0, 27 }, { 0, 36 }, { 0, 45 }, { 0, 54 }, { 0, 63 }, { 0, 72 },
			{ 9, 27 }, { 9, 36 }, { 9, 45 }, { 9, 54 }, { 9, 63 }, { 9, 72 },
			{ 18, 27 }, { 18, 36 }, { 18, 45 }, { 18, 54 }, { 18, 63 }, { 18, 72 },
			{ 27, 54 }, { 27, 63 }, { 27, 72 },
			{ 36, 54 }, { 36, 63 }, { 36, 72 },
			{ 45, 54 }, { 45, 63 }, { 45, 72 }
		};

		var combinations = new List<(CellMap, IReadOnlyList<(int, int)>, int)>();

		// Initializes fit types.
		for (var j = 0; j < 3; j++)
		{
			for (int i = 0, length = fitTableRow.Length >> 1; i < length; i++)
			{
				int c11 = fitTableRow[i, 0] + j * 27, c21 = fitTableRow[i, 1] + j * 27;
				int c12 = c11 + 9, c22 = c21 + 9;
				int c13 = c11 + 18, c23 = c21 + 18;
				combinations.Add(
					(
						CellMap.Empty + c11 + c12 + c13 + c21 + c22 + c23,
						new[] { (c11, c21), (c12, c22), (c13, c23) },
						3
					)
				);
			}
		}
		for (var j = 0; j < 3; j++)
		{
			for (int i = 0, length = fitTableColumn.Length >> 1; i < length; i++)
			{
				int c11 = fitTableColumn[i, 0] + j * 3, c21 = fitTableColumn[i, 1] + j * 3;
				int c12 = c11 + 1, c22 = c21 + 1;
				int c13 = c11 + 2, c23 = c21 + 2;
				combinations.Add(
					(
						CellMap.Empty + c11 + c12 + c13 + c21 + c22 + c23,
						new[] { (c11, c21), (c12, c22), (c13, c23) },
						3
					)
				);
			}
		}

		// Initializes fat types.
		for (var size = 3; size <= 7; size++)
		{
			for (int i = 0, length = houses.Length >> 1; i < length; i++)
			{
				int house1 = houses[i, 0], house2 = houses[i, 1];
				foreach (short mask in new BitSubsetsGenerator(9, size))
				{
					// Check whether all cells are in same house.
					// If so, continue the loop immediately.
					if (size == 3 && (mask >> 6 == 7 || (mask >> 3 & 7) == 7 || (mask & 7) == 7))
					{
						continue;
					}

					var map = CellMap.Empty;
					var pairs = new List<(int, int)>();
					foreach (var pos in mask)
					{
						int cell1 = HouseCells[house1][pos], cell2 = HouseCells[house2][pos];
						map.Add(cell1);
						map.Add(cell2);
						pairs.Add((cell1, cell2));
					}

					combinations.Add((map, pairs, size));
				}
			}
		}

		PatternInfos = combinations;
	}
}
