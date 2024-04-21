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
		DirectHiddenSubsetMaxSize = 4,
		DirectNakedSubsetMaxSize = 4
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
		var accumulator = new List<Step>();
		if (dfs(ref context, accumulator, in grid, [], []) is { } step)
		{
			return step;
		}

		// Sort and remove duplicate instances if worth.
		var sortedList = StepMarshal.RemoveDuplicateItems(accumulator).ToList();
		StepMarshal.SortItems(sortedList);

		if (context.OnlyFindOne)
		{
			return sortedList[0];
		}

		context.Accumulator.AddRange(sortedList);
		return null;


		static ComplexSingleStep? dfs(
			scoped ref AnalysisContext context,
			List<Step> accumulator,
			scoped ref readonly Grid grid,
			LinkedList<Step> interimSteps,
			List<Step> previousIndirectFoundSteps
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

			// Remove possible steps that has already been recorded into previosly found steps.
			foreach (var step in indirectFoundSteps[..])
			{
				if (previousIndirectFoundSteps.Contains(step))
				{
					indirectFoundSteps.Remove(step);
				}
			}
			if (indirectFoundSteps.Count == 0)
			{
				// Nothing can be found.
				return null;
			}

			// Record all steps that may not duplicate.
			previousIndirectFoundSteps.AddRange(indirectFoundSteps);

			// Iterate on each step collected, and check whether it can be solved with direct singles.
			foreach (var indirectStep in indirectFoundSteps)
			{
				// Check whether the step is valid.
				// A step will be valid if:
				//   1) The step has new conclusions that recorded steps don't have.
				//   2) The step becomes valid if at least one record step indeed influences the current step.
				var isValid = true;
				foreach (var interimStep in interimSteps)
				{
					if (interimStep.ConclusionText == indirectStep.ConclusionText)
					{
						isValid = false;
						break;
					}

					var (digitsMaskInterim, houseInterim) = interimStep switch
					{
						LockedCandidatesStep { Digit: var digit, BaseSet: var set } => ((Mask)(1 << digit), set),
						NakedSubsetStep { DigitsMask: var digitsMask, House: var set } => (digitsMask, set),
						HiddenSubsetStep { DigitsMask: var digitsMask, House: var set } => (digitsMask, set)
					};
					var (digitsMaskIndirect, houseIndirect) = indirectStep switch
					{
						LockedCandidatesStep { Digit: var digit, BaseSet: var set } => ((Mask)(1 << digit), set),
						NakedSubsetStep { DigitsMask: var digitsMask, House: var set } => (digitsMask, set),
						HiddenSubsetStep { DigitsMask: var digitsMask, House: var set } => (digitsMask, set)
					};
					if ((digitsMaskInterim & digitsMaskIndirect) != 0 && houseInterim == houseIndirect)
					{
						isValid = true;
						break;
					}
				}
				if (!isValid)
				{
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
						var views = new View[interimSteps.Count + 1];
						var tempConclusions = new List<Conclusion>();
						var tempIndex = 0;
						foreach (var interimStep in interimSteps)
						{
							tempConclusions.AddRange(interimStep.Conclusions.AsReadOnlySpan());
							views[tempIndex++] = [
								.. interimStep.Views![0],
								..
								from conclusion in tempConclusions
								select new CandidateViewNode(ColorIdentifier.Elimination, conclusion.Candidate)
							];
						}
						views[tempIndex] = [
							.. directStep.Views![0],
							..
							from conclusion in tempConclusions
							select new CandidateViewNode(ColorIdentifier.Elimination, conclusion.Candidate)
						];

						// Add step into accumulator or return step.
						var step = new ComplexSingleStep(
							directStep.Conclusions,
							views,
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

						accumulator.Add(step);
						return null;
					}
				}

				// If code goes to here, the puzzle won't be solved with the current step.
				// We should continue the searching from the current state.
				// Use this puzzle to check for the next elimination step by recursion.
				if (dfs(ref context, accumulator, in playground, interimSteps, previousIndirectFoundSteps) is { } finalStep)
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
