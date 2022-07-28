namespace Sudoku.Solving.Manual.Searchers;

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
}
