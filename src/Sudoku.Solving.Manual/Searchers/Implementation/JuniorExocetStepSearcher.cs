namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed partial class JuniorExocetStepSearcher : IJuniorExocetStepSearcher
{
	/// <inheritdoc/>
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// TODO: Re-implement JE.
		return null;
	}
}
