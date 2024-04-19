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
	private static readonly DirectIntersectionStepSearcher IntersectionSearcher = new()
	{
		AllowDirectPointing = true,
		AllowDirectClaiming = true
	};

	/// <summary>
	/// Indicates the searcher for subset.
	/// </summary>
	private static readonly DirectSubsetStepSearcher SubsetSearcher = new()
	{
		AllowDirectHiddenSubset = true,
		AllowDirectLockedHiddenSubset = true,
		AllowDirectLockedSubset = true,
		AllowDirectNakedSubset = true,
		DirectHiddenSubsetMaxSize = 2,
		DirectNakedSubsetMaxSize = 2
	};

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
		scoped ref readonly var grid = ref context.Grid;
		return dfs(ref context, in grid, []) is { } step ? step : null;


		static ComplexSingleStep? dfs(
			scoped ref AnalysisContext context,
			scoped ref readonly Grid grid,
			LinkedList<Step> interimSteps
		)
		{
			// Collect all steps by using indirect techniques.
			var indirectFoundSteps = new List<Step>();
			scoped var tempContext = new AnalysisContext(
				indirectFoundSteps,
				in grid,
				in Grid.NullRef,
				false,
				context.IsSukaku,
				context.PredefinedOptions
			);
			LockedCandidatesSearcher.Collect(ref tempContext);
			LockedSubsetSearcher.Collect(ref tempContext);
			NormalSubsetSearcher.Collect(ref tempContext);

			if (indirectFoundSteps.Count == 0)
			{
				// Nothing can be found.
				return null;
			}

			// Iterate on each step collected, and check whether it can be solved with direct singles.
			foreach (var indirectStep in indirectFoundSteps)
			{
				if (interimSteps.Exists(step => step.ConclusionText == indirectStep.ConclusionText))
				{
					// Skips for recorded steps.
					continue;
				}

				// Push.
				interimSteps.AddLast(indirectStep);
				var playground = grid;
				playground.Apply(indirectStep);

				// Check whether the puzzle can be solved via a direct single.
				var directStepsFound = new List<Step>();
				scoped var nestedContext = new AnalysisContext(
					directStepsFound,
					in playground,
					in Grid.NullRef,
					false,
					context.IsSukaku,
					context.PredefinedOptions
				);
				IntersectionSearcher.Collect(ref nestedContext);
				SubsetSearcher.Collect(ref nestedContext);

				if (directStepsFound.Count != 0)
				{
					// Good! We have already found a step available! Iterate on each step to create the result value.
					foreach (ComplexSingleBaseStep directStep in directStepsFound)
					{
						// Add step into accumulator or return step.
						var step = new ComplexSingleStep(
							directStep.Conclusions,
							[
								..
								from interimStep in interimSteps
								let interimConclusion =
									from conclusion in interimStep.Conclusions
									select new CandidateViewNode(ColorIdentifier.Elimination, conclusion.Candidate)
								select (View)([.. interimStep.Views![0], .. interimConclusion]),
								.. directStep.Views
							],
							context.PredefinedOptions,
							directStep.Cell,
							directStep.Digit,
							directStep.Subtype,
							directStep.BasedOn,
							[.. from interimStep in interimSteps select (Technique[])[interimStep.Code]]
						);
						if (context.OnlyFindOne)
						{
							return step;
						}

						context.Accumulator.Add(step);
						return null;
					}
				}

				// If code goes to here, the puzzle won't be solved with the current step.
				// We should continue the searching from the current state.
				// Use this puzzle to check for the next elimination step by recursion.
				if (dfs(ref context, in playground, interimSteps) is { } finalStep)
				{
					return finalStep;
				}

				// Pop.
				interimSteps.RemoveLast();
				playground = grid;
			}

			// None found. Return null.
			return null;
		}
	}
}
