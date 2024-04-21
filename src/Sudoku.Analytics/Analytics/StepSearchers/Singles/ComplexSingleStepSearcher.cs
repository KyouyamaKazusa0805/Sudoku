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
	private readonly DirectIntersectionStepSearcher _searcher_DirectLockedCandidates = new()
	{
		AllowDirectPointing = true,
		AllowDirectClaiming = true
	};

	/// <summary>
	/// Indicates the searcher for subset.
	/// </summary>
	private readonly DirectSubsetStepSearcher _searcher_DirectSubset = new()
	{
		AllowDirectHiddenSubset = true,
		AllowDirectLockedHiddenSubset = true,
		AllowDirectLockedSubset = true,
		AllowDirectNakedSubset = true
	};

	/// <summary>
	/// Indicates the searcher for locked candidates.
	/// </summary>
	private readonly LockedCandidatesStepSearcher _searcher_LockedCandidates = new();

	/// <summary>
	/// Indicates the searcher for locked subsets.
	/// </summary>
	private readonly LockedSubsetStepSearcher _searcher_LockedSubset = new();

	/// <summary>
	/// Indicates the searcher for normal subsets.
	/// </summary>
	private readonly NormalSubsetStepSearcher _searcher_Subset = new();


	/// <summary>
	/// Indicates the maximum size of naked subsets. The maximum value is 4.
	/// </summary>
	[SettingItemName(SettingItemNames.NakedSubsetMaxSizeInComplexSingle)]
	public int NakedSubsetMaxSize { get; set; } = 4;

	/// <summary>
	/// Indicates the maximum size of hidden subsets. The maximum value is 4.
	/// </summary>
	[SettingItemName(SettingItemNames.HiddenSubsetMaxSizeInComplexSingle)]
	public int HiddenSubsetMaxSize { get; set; } = 4;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Recursively searching for all possible steps.
		var accumulator = new List<Step>();
		entry(ref context, accumulator, in context.Grid);

		// Remove steps that don't satisfy the size limit.
		var stepsSatisfied = new List<Step>();
		foreach (ComplexSingleStep step in accumulator)
		{
			var flag = true;
			foreach (var technique in from techniquesGroup in step.IndirectTechniques from technique in techniquesGroup select technique)
			{
				if (technique.GetGroup() == TechniqueGroup.Subset)
				{
					var (size, isHidden) = technique switch
					{
						Technique.NakedPair or Technique.NakedPairPlus or Technique.LockedPair => (2, false),
						Technique.HiddenPair or Technique.LockedHiddenPair => (2, true),
						Technique.NakedTriple or Technique.NakedTriplePlus or Technique.LockedTriple => (3, false),
						Technique.HiddenTriple or Technique.LockedHiddenTriple => (3, true),
						Technique.NakedQuadruple or Technique.NakedQuadruplePlus => (4, false),
						Technique.HiddenQuadruple => (4, true)
					};
					if (isHidden && size > HiddenSubsetMaxSize || !isHidden && size > NakedSubsetMaxSize)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				stepsSatisfied.Add(step);
			}
		}

		// Sort instances if worth.
		// We don't remove duplicate items because the searcher may not produce same steps,
		// and the corresponding step type doesn't override method 'Equals'.
		if (stepsSatisfied.Count == 0)
		{
			return null;
		}

		StepMarshal.SortItems(stepsSatisfied);
		if (context.OnlyFindOne)
		{
			return stepsSatisfied[0];
		}

		context.Accumulator.AddRange(stepsSatisfied);
		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void entry(scoped ref AnalysisContext context, List<Step> accumulator, scoped ref readonly Grid grid)
			=> dfs(ref context, accumulator, in grid, [], []);

		void dfs(
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
			_searcher_LockedSubset.Collect(ref tempContext);
			_searcher_LockedCandidates.Collect(ref tempContext);
			_searcher_Subset.Collect(ref tempContext);

			// Remove possible steps that has already been recorded into previously found steps.
			// During the searching, we may found a step that may be appeared in the previous grid state,
			// meaning we can found a step in both current state and one of its sub-grid state, which causes a redudant searching
			// if we don't apply them at parent state.
			// We should ignore them in child branches, guaranteeing such steps will be applied in the first meet.
			// However, this limit will produce a potential bug - if two steps are not relative,
			// the searcher will ignore the second one aggressively, but the final assignment will use both,
			// meaning we have removed a step that may be a key one. One example is this:
			//
			//     0002+471630+6+300+500041039+6+500001000+60568+97+51+234500+600900000063052000000+30+6326508000
			//
			// Here the puzzle will use two locked candidates of digit 4 and 9 in r9b9. But both of them can be found in the first step.
			// If we use the first one (i.e. locked candidates of digit 4), the second one (i.e. locked candidates of digit 9) will be ignored
			// and no longer in use.
			// To fix the bug, we should apply both of steps in one same grid state.
			if (indirectFoundSteps.Count != 0)
			{
				foreach (var step in indirectFoundSteps[..])
				{
					if (previousIndirectFoundSteps.Contains(step))
					{
						indirectFoundSteps.Remove(step);
					}
				}
			}
			if (indirectFoundSteps.Count == 0)
			{
				// Nothing can be found.
				return;
			}

			// Record all steps that may not duplicate.
			previousIndirectFoundSteps.AddRange(indirectFoundSteps);

			// Iterate on each step collected, and check whether it can be solved with direct singles.
			foreach (var indirectStep in indirectFoundSteps)
			{
				// A step will be valid if it has new conclusions that recorded steps don't have.
				var isValid = true;
				foreach (var interimStep in interimSteps)
				{
					if (interimStep.ConclusionText == indirectStep.ConclusionText)
					{
						isValid = false;
						break;
					}
				}
				if (!isValid)
				{
					continue;
				}

				pushStep(out var playground, in grid, indirectStep, interimSteps);

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
				_searcher_DirectLockedCandidates.Collect(ref nestedContext);
				_searcher_DirectSubset.Collect(ref nestedContext);

				if (directStepsFound.Count != 0)
				{
					// Good! We have already found a step available! Iterate on each step to create the result value.
					foreach (ComplexSingleBaseStep directStep in directStepsFound)
					{
						var (views, tempConclusions, tempIndex) = (new View[interimSteps.Count + 1], new List<Conclusion>(), 0);
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
						accumulator.Add(
							new ComplexSingleStep(
								directStep.Conclusions,
								views,
								context.PredefinedOptions,
								directStep.Cell,
								directStep.Digit,
								directStep.Subtype,
								directStep.BasedOn,
								[.. from interimStep in interimSteps select (Technique[])[interimStep.Code]]
							)
						);
						goto PopStep;
					}
				}

				// If code goes to here, the puzzle won't be solved with the current step.
				// We should continue the searching from the current state.
				// Use this puzzle to check for the next elimination step by recursion.
				dfs(ref context, accumulator, in playground, interimSteps, previousIndirectFoundSteps);

			PopStep:
				popStep(ref playground, in grid, interimSteps);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void pushStep(out Grid playground, scoped ref readonly Grid baseGrid, Step indirectStep, LinkedList<Step> interimSteps)
		{
			interimSteps.AddLast(indirectStep);
			playground = baseGrid;
			playground.Apply(indirectStep);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void popStep(scoped ref Grid playground, scoped ref readonly Grid baseGrid, LinkedList<Step> interimSteps)
		{
			interimSteps.RemoveLast();
			playground = baseGrid;
		}
	}
}
