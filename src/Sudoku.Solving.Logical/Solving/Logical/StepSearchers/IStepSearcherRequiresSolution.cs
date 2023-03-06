namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Indicates a step searcher that requires a solution grid for the technique searching usages.
/// </summary>
public interface IStepSearcherRequiresSolution : IStepSearcher
{
	/// <summary>
	/// Indicates the reference of the solution sudoku grid.
	/// </summary>
	Grid Solution { get; set; }
}
