namespace Sudoku.Solving.Logical.Prototypes;

/// <summary>
/// Provides with a <b>Chromatic Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Chromatic Pattern type 1</item>
/// <!--
/// <item>Chromatic Pattern type 2</item>
/// <item>Chromatic Pattern type 3</item>
/// <item>Chromatic Pattern type 4</item>
/// -->
/// </list>
/// </item>
/// <item>
/// Extended types:
/// <list type="bullet">
/// <item>Chromatic Pattern XZ</item>
/// </list>
/// </item>
/// </list>
/// </summary>
/// <remarks>
/// For more information about a "chromatic pattern",
/// please visit <see href="http://forum.enjoysudoku.com/chromatic-patterns-t39885.html">this link</see>.
/// </remarks>
public interface IChromaticPatternStepSearcher : INegativeRankStepSearcher
{
	/// <summary>
	/// The possible pattern offsets.
	/// </summary>
	protected static readonly (int[], int[], int[], int[])[] PatternOffsets;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IChromaticPatternStepSearcher()
	{
		int[][] diagonalCases = { new[] { 0, 10, 20 }, new[] { 1, 11, 18 }, new[] { 2, 9, 19 } };
		int[][] antidiagonalCases = { new[] { 0, 11, 19 }, new[] { 1, 9, 20 }, new[] { 2, 10, 18 } };

		var patternOffsetsList = new List<(int[], int[], int[], int[])>();
		foreach (var (aCase, bCase, cCase, dCase) in stackalloc[]
		{
			(true, false, false, false),
			(false, true, false, false),
			(false, false, true, false),
			(false, false, false, true)
		})
		{
			// Phase 1.
			foreach (var a in aCase ? diagonalCases : antidiagonalCases)
			{
				foreach (var b in bCase ? diagonalCases : antidiagonalCases)
				{
					foreach (var c in cCase ? diagonalCases : antidiagonalCases)
					{
						foreach (var d in dCase ? diagonalCases : antidiagonalCases)
						{
							patternOffsetsList.Add((a, b, c, d));
						}
					}
				}
			}

			// Phase 2.
			foreach (var a in aCase ? antidiagonalCases : diagonalCases)
			{
				foreach (var b in bCase ? antidiagonalCases : diagonalCases)
				{
					foreach (var c in cCase ? antidiagonalCases : diagonalCases)
					{
						foreach (var d in dCase ? antidiagonalCases : diagonalCases)
						{
							patternOffsetsList.Add((a, b, c, d));
						}
					}
				}
			}
		}

		PatternOffsets = patternOffsetsList.ToArray();
	}
}
