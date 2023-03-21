namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// (<b>Not implemented</b>) Provides with a <b>Senior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class SeniorExocetStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
