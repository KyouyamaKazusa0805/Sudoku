namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for bi-value oddagon steps.
/// </summary>
public interface IBivalueOddagonStepSearcher : IRankTheoryStepSearcher, IUniqueLoopOrBivalueOddagonStepSearcher
{
}
