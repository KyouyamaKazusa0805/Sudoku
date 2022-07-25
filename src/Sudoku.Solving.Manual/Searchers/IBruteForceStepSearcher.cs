namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for brute force steps.
/// </summary>
public interface IBruteForceStepSearcher : ILastResortStepSearcher, IStepSearcherRequiresSolution
{
}
