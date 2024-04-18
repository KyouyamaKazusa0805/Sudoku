namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Complex Single</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Complex Full House</item>
/// <item>Complex Hidden Single</item>
/// <item>Complex Naked Single</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_ComplexSingleStepSearcher",
	Technique.ComplexFullHouse, Technique.ComplexCrosshatchingBlock, Technique.ComplexCrosshatchingRow,
	Technique.ComplexCrosshatchingColumn, Technique.ComplexNakedSingle,
	IsAvailabilityReadOnly = true,
	IsOrderingFixed = true,
	RuntimeFlags = StepSearcherRuntimeFlags.DirectTechniquesOnly)]
public sealed partial class ComplexSingleStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the searcher for intersection.
	/// </summary>
	private static readonly DirectIntersectionStepSearcher IntersectionSearcher = new();

	/// <summary>
	/// Indicates the searcher for subset.
	/// </summary>
	private static readonly DirectSubsetStepSearcher SubsetSearcher = new();

	/// <summary>
	/// Indicates the searcher for locked candidates.
	/// </summary>
	private static readonly LockedCandidatesStepSearcher LockedCandidatesSearcher = new();

	/// <summary>
	/// Indicates the searcher for locked subsets.
	/// </summary>
	private static readonly LockedSubsetStepSearcher LockedSubsetSearcher = new();

	/// <summary>
	/// Indicates the searcher for normal subsets.
	/// </summary>
	private static readonly NormalSubsetStepSearcher NormalSubsetSearcher = new();


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{


		return null;
	}
}
