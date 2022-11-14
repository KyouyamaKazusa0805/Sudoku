namespace Sudoku.Solving.Logical.StepSearchers.Specialized;

/// <summary>
/// Defines a special step searcher that can gather all possible steps of various techniques, in a step.
/// </summary>
public interface IStepGatherableSearcher
{
	/// <summary>
	/// Indicates the maximum steps can be gathered.
	/// </summary>
	int MaxStepsGathered { get; set; }


	/// <summary>
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="puzzle">The puzzle grid.</param>
	/// <param name="progress">The progress instance that is used for reporting the status.</param>
	/// <param name="cancellationToken">The cancellation token used for canceling an operation.</param>
	/// <returns>The result.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	IEnumerable<IStep> Search(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
