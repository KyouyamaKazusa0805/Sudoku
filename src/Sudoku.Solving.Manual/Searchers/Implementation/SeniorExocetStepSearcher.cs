namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed partial class SeniorExocetStepSearcher : ISeniorExocetStepSearcher
{
	/// <inheritdoc/>
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
