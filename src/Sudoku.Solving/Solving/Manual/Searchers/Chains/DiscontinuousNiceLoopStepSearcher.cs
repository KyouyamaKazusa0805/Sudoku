using Sudoku.Collections;
using Sudoku.Presentation;

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
public sealed unsafe class DiscontinuousNiceLoopStepSearcher : IDiscontinuousNiceLoopStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(13, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne) =>
		throw new NotImplementedException();
}
