namespace Sudoku.Solving.Manual;

partial class ManualSolver
{
	/// <summary>
	/// Solve the puzzle with <see cref="AnalyzeDifficultyStrictly"/> option.
	/// </summary>
	/// <param name="this">The current manual solver instance.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="cloneation">The cloneation grid to calculate.</param>
	/// <param name="steps">All steps found.</param>
	/// <param name="solution">The solution.</param>
	/// <param name="sukaku">Indicates whether the current mode is sukaku mode.</param>
	/// <param name="progressResult">
	/// (<see langword="ref"/> parameter)
	/// The progress result. This parameter is used for modify the state of UI controls.
	/// The current argument won't be used until <paramref name="progress"/> isn't <see langword="null"/>.
	/// In the default case, this parameter being
	/// <see langword="default"/>(<see cref="GridProgressResult"/>) is okay.
	/// </param>
	/// <param name="progress">
	/// The progress used for report the current state. If we don't need, the value should
	/// be assigned <see langword="null"/>.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that is used to cancel the operation.</param>
	/// <returns>The analysis result.</returns>
	/// <exception cref="WrongStepException">
	/// Throws when the solver can't solved due to wrong handling.
	/// </exception>
	/// <seealso cref="GridProgressResult"/>
	private static unsafe AnalysisResult SolveSeMode(
		ManualSolver @this, in SudokuGrid grid, ref SudokuGrid cloneation, IList<StepInfo> steps, in SudokuGrid solution,
		bool sukaku, ref GridProgressResult progressResult, IProgress<IProgressResult>? progress,
		CancellationToken? cancellationToken)
	{
		var searchers = @this.GetSeModeSearchers(solution);
		var stepGrids = new List<SudokuGrid>();
		var bag = new List<StepInfo>();
		var stopwatch = new Stopwatch();
		stopwatch.Start();

	Restart:
		InitializeMaps(cloneation);
		for (int i = 0, length = searchers.Length; i < length; i++)
		{
			var searcherListGroup = searchers[i];
			foreach (var searcher in searcherListGroup)
			{
				if (sukaku && searcher is UniquenessStepSearcher)
				{
					// Sukaku mode can't use them.
					// In fact, sukaku can use uniqueness tests, however the program should
					// produce a large modification.
					continue;
				}

				if (TechniqueProperties.FromSearcher(searcher) is not { IsEnabled: true })
				{
					continue;
				}

				if (@this.EnableGarbageCollectionForcedly)
				{
					GC.Collect();
				}

				searcher.GetAll(bag, cloneation);
			}
			if (bag.Count == 0)
			{
				continue;
			}

			if (@this.FastSearch)
			{
				decimal minDiff = bag.Min(static info => info.Difficulty);
				var selection = from info in bag where info.Difficulty == minDiff select info;
				if (!selection.Any())
				{
					continue;
				}

				bool allConclusionsAreValid = true;
				foreach (var element in selection)
				{
					if (!CheckConclusionsValidity(solution, element.Conclusions))
					{
						allConclusionsAreValid = false;
						break;
					}
				}

				if (!@this.CheckConclusionValidityAfterSearched || allConclusionsAreValid)
				{
					foreach (var step in selection)
					{
						if (
							@this.RecordStep(
								steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result,
								cancellationToken
							)
						)
						{
							stopwatch.Stop();
							return result;
						}
					}

					// The puzzle has not been finished,
					// we should turn to the first step finder
					// to continue solving puzzle.
					bag.Clear();

					if (progress is not null)
					{
						ReportProgress(cloneation, progress, ref progressResult);
					}

					goto Restart;
				}
				else
				{
					var solutionCopied = solution;
					throw new WrongStepException(grid, selection.First(first));

					bool first(StepInfo step) => !CheckConclusionsValidity(solutionCopied, step.Conclusions);
				}
			}
			else
			{
				var step = (from info in bag orderby info.Difficulty select info).FirstOrDefault();
				if (step is null)
				{
					// If current step can't find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (!@this.CheckConclusionValidityAfterSearched
					|| CheckConclusionsValidity(solution, step.Conclusions))
				{
					if (
						@this.RecordStep(
							steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result,
							cancellationToken
						)
					)
					{
						// The puzzle has been solved.
						// :)
						stopwatch.Stop();
						return result;
					}
					else
					{
						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Restart;
					}
				}
				else
				{
					throw new WrongStepException(grid, step);
				}
			}
		}

		// All solver can't finish the puzzle...
		// :(
		stopwatch.Stop();
		return new(@this.SolverName, grid, false, stopwatch.Elapsed)
		{
			Steps = steps.AsReadOnlyList(),
			StepGrids = stepGrids,
		};
	}
}
