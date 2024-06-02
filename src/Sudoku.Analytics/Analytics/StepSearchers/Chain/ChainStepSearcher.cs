namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Alternating Inference Chains</item>
/// <item>Continuous Nice Loops</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_ChainStepSearcher",
	Technique.XChain, Technique.YChain, Technique.AlternatingInferenceChain, Technique.ContinuousNiceLoop, Technique.DiscontinuousNiceLoop,
	Technique.XyXChain, Technique.XyChain, Technique.FishyCycle, Technique.PurpleCow)]
public sealed partial class ChainStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		return null;
#if false
		ref readonly var grid = ref context.Grid;

		// Step 1: Collect for all strong and weak links appeared in the grid.
		var strongLinks = new Dictionary<Node, List<Node>>();
		var weakLinks = new Dictionary<Node, List<Node>>();
#endif
	}
}
