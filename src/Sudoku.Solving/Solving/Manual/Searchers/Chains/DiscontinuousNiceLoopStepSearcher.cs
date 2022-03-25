using Sudoku.Collections;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Discontinuous Nice Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Irregular Wings</item>
/// <item>Alternating Inference Chain</item>
/// </list>
/// </summary>
[StepSearcher]
[SeparatedStepSearcher(0, nameof(XEnabled), true, nameof(YEnabled), false)]
[SeparatedStepSearcher(1, nameof(XEnabled), true, nameof(YEnabled), true)]
public sealed class DiscontinuousNiceLoopStepSearcher : IDiscontinuousNiceLoopStepSearcher
{
	/// <summary>
	/// Indicates whether the X-chain is enabled.
	/// </summary>
	public bool XEnabled { get; init; }

	/// <summary>
	/// Indicates whether the Y-Chain is enabled.
	/// </summary>
	public bool YEnabled { get; init; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(13, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne) =>
		throw new NotImplementedException();
}
