namespace Sudoku.Solving.Implementations.Searcher;

[StepSearcher]
internal sealed partial class SeniorExocetStepSearcher : ISeniorExocetStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
