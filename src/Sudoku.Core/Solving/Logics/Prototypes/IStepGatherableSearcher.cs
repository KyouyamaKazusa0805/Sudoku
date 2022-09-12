namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Defines a special step searcher that can gather all possible steps of various techniques, in a step.
/// </summary>
public interface IStepGatherableSearcher
{
	/// <summary>
	/// Search for all possible steps in a grid, and group them up by its technique name.
	/// </summary>
	/// <param name="puzzle"><inheritdoc cref="Search(in Grid, CancellationToken)" path="/param[@name='puzzle']"/></param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Search(in Grid, CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns>The result grouped by technique names.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	public sealed IEnumerable<IGrouping<string, IStep>> SearchAndGroupByName(scoped in Grid puzzle, CancellationToken cancellationToken = default)
		=> from step in Search(puzzle, cancellationToken) group step by step.Name;

	/// <summary>
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="puzzle">The puzzle grid.</param>
	/// <param name="cancellationToken">The cancellation token used for canceling an operation.</param>
	/// <returns>The result.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	public abstract IEnumerable<IStep> Search(scoped in Grid puzzle, CancellationToken cancellationToken = default);
}
