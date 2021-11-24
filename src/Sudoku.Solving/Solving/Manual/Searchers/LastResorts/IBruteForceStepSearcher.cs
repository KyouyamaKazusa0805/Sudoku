namespace Sudoku.Solving.Manual.Searchers.LastResorts;

/// <summary>
/// Defines a step searcher that searches for brute force steps.
/// </summary>
public interface IBruteForceStepSearcher : ILastResortStepSearcher
{
	/// <summary>
	/// Indicates the solution grid. The grid will be used for validating the result.
	/// </summary>
	Grid Solution { get; set; }
}
