namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Brute Force</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Brute Force</item>
/// </list>
/// </summary>
public interface IBruteForceStepSearcher : ILastResortStepSearcher, IStepSearcherRequiresSolution
{
}
