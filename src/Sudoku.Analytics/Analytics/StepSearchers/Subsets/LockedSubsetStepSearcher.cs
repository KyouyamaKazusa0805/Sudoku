namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Locked Subset</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Locked Pair</item>
/// <item>Locked Hidden Pair</item>
/// <item>Locked Triple</item>
/// <item>Locked Hidden Triple</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.LockedPair, Technique.LockedTriple, Technique.LockedHiddenPair, Technique.LockedHiddenTriple,
	Technique.NakedPairPlus, Technique.NakedTriplePlus, Technique.NakedQuadruplePlus)]
[StepSearcherRuntimeName("StepSearcherName_LockedSubsetStepSearcher")]
public sealed partial class LockedSubsetStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context) => SubsetModule.CollectCore(true, ref context);
}
