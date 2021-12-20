namespace Sudoku.Solving.Manual.Searchers.Exocets;

/// <summary>
/// Provides with a <b>Senior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class SeniorExocetStepSearcher : ISeniorExocetStepSearcher
{
	/// <inheritdoc/>
	public bool CheckAdvanced { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(32, DisplayingLevel.D);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
