namespace Sudoku.Solving.Manual.Searchers.RankTheory;

/// <summary>
/// Defines a step searcher that searches for bi-value oddagon steps.
/// </summary>
public interface IBivalueOddagonStepSearcher : IRankTheoryStepSearcher, IUniqueLoopOrBivalueOddagonStepSearcher
{
}
