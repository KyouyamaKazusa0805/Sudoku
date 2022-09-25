namespace Sudoku.Solving.Logical.Implementations.Searchers;

[StepSearcher]
internal sealed partial class SeniorExocetStepSearcher : ISeniorExocetStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
