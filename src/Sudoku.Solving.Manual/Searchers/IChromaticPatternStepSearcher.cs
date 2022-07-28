namespace Sudoku.Solving.Manual.Searchers;

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
public interface IChromaticPatternStepSearcher : IRankTheoryStepSearcher
{
}
