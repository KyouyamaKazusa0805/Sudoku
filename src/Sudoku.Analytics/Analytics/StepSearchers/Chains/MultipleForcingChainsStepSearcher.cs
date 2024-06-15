namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Multiple Forcing Chains</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Cell Forcing Chains</item>
/// <item>Region Forcing Chains (i.e. House Forcing Chains)</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_MultipleForcingChainsStepSearcher",
	Technique.CellForcingChains, Technique.RegionForcingChains)]
public sealed partial class MultipleForcingChainsStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		return null;
	}
}
