namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Locked Subset</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Naked Subsets (+):
/// <list type="bullet">
/// <item>Naked Pair (+)</item>
/// <item>Naked Triple (+)</item>
/// <item>Naked Quadruple (+)</item>
/// </list>
/// </item>
/// <item>
/// Naked Subsets:
/// <list type="bullet">
/// <item>Naked Pair</item>
/// <item>Naked Triple</item>
/// <item>Naked Quadruple</item>
/// </list>
/// </item>
/// <item>
/// Hidden Subsets:
/// <list type="bullet">
/// <item>Hidden Pair</item>
/// <item>Hidden Triple</item>
/// <item>Hidden Quadruple</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_NormalSubsetStepSearcher",
	Technique.HiddenPair, Technique.HiddenTriple, Technique.HiddenQuadruple,
	Technique.NakedPair, Technique.NakedTriple, Technique.NakedQuadruple,
	IsCachingSafe = true)]
public sealed partial class NormalSubsetStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the max naked subset size to be searched.
	/// </summary>
	internal int MaxNakedSubsetSize { get; set; } = 4;

	/// <summary>
	/// Indicates the max hidden subset size to be searched.
	/// </summary>
	internal int MaxHiddenSubsetSize { get; set; } = 4;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
		=> SubsetModule.CollectCore(false, ref context, MaxNakedSubsetSize, MaxHiddenSubsetSize);
}
