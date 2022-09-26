namespace Sudoku.Solving.Logical.StepSearchers.Specialized;

/// <summary>
/// Indicates a step searcher that requires a solution grid for the technique searching usages.
/// </summary>
public interface IStepSearcherRequiresSolution : IStepSearcher
{
	/// <summary>
	/// Indicates the reference of the solution sudoku grid.
	/// </summary>
	public abstract Grid Solution { get; set; }
}
