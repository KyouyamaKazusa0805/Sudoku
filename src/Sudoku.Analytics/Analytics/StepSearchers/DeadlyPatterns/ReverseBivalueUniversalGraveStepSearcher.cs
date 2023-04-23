namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Reverse Bi-value Universal Grave</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Reverse Unique Rectangle</item>
/// <item>Reverse Unique Loop</item>
/// <item>Reverse Bivalue Universal Grave (Separated Type)</item>
/// </list>
/// </summary>
[StepSearcher, ConditionalCases(ConditionalCase.Standard)]
public sealed partial class ReverseBivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// TODO: Will be implemented later.
		return null;
	}
}
