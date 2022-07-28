namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Unique Square</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Square Type 1</item>
/// <item>Unique Square Type 2</item>
/// <item>Unique Square Type 3</item>
/// <item>Unique Square Type 4</item>
/// </list>
/// </summary>
public interface IUniqueSquareStepSearcher : IDeadlyPatternStepSearcher
{
}
