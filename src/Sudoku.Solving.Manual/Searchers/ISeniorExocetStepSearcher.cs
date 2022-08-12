namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Senior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
public interface ISeniorExocetStepSearcher : IExocetStepSearcher
{
}

[StepSearcher]
internal sealed partial class SeniorExocetStepSearcher : ISeniorExocetStepSearcher
{
	/// <inheritdoc/>
	[SearcherProperty]
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
