namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Unique Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item></item>
/// </list>
/// </summary>
internal sealed class UniqueLoopStepSearcher : IUniqueLoopStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(10, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		if (BivalueMap.Count < 6)
		{
			goto ReturnNull;
		}


	ReturnNull:
		return null;
	}
}
