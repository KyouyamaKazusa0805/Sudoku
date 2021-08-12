namespace Sudoku.Solving.Manual;

partial class ManualSolver
{
	/// <summary>
	/// Solve naively.
	/// </summary>
	/// <param name="this">The current manual solver.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="cloneation">The cloneation grid to calculate.</param>
	/// <param name="steps">All steps found.</param>
	/// <param name="solution">The solution.</param>
	/// <param name="sukaku">Indicates whether the current mode is sukaku.</param>
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
	private static unsafe AnalysisResult SolveNaively(
		ManualSolver @this, in SudokuGrid grid, ref SudokuGrid cloneation, IList<StepInfo> steps,
		in SudokuGrid solution, bool sukaku, ref GridProgressResult progressResult,
		IProgress<IProgressResult>? progress, CancellationToken? cancellationToken)
	{
		// Check symmetry first.
		var stepGrids = new List<SudokuGrid>();
		if (!sukaku && @this.CheckGurthSymmetricalPlacement)
		{
			var symmetrySearcher =
#if I_WANT_TO_DISABLE_THIS__BECAUSE_OF_TOO_SLOW
				new Gsp2StepSearcher();
#else
				new GspStepSearcher();
#endif
			if (symmetrySearcher.GetOne(cloneation) is { } tempStep)
			{
				if (CheckConclusionsValidity(solution, tempStep.Conclusions))
				{
					stepGrids.Add(cloneation);
					tempStep.ApplyTo(ref cloneation);
					steps.Add(tempStep);

					if (progress is not null)
					{
						ReportProgress(cloneation, progress, ref progressResult);
					}
				}
				else
				{
					throw new WrongStepException(grid, tempStep);
				}
			}
		}

		// Start searching.
		var searchers = @this.GetHodokuModeSearchers(solution);
		if (@this.UseCalculationPriority)
		{
			searchers.Sort(&cmp);
		}

		var bag = new List<StepInfo>();
		var stopwatch = new Stopwatch();
		stopwatch.Start();

	Restart:
		InitializeMaps(cloneation);
		for (int i = 0, length = searchers.Length; i < length; i++)
		{
			var searcher = searchers[i];
			if (sukaku && searcher is UniquenessStepSearcher)
			{
				continue;
			}

			bool isEnabled = TechniqueProperties.FromSearcher(searcher)!.IsEnabled;
			if (!isEnabled)
			{
				continue;
			}

			searcher.GetAll(bag, cloneation);
			if (bag.Count == 0)
			{
				continue;
			}

			if (@this.FastSearch)
			{
				bool allConclusionsAreValid = true;
				foreach (var element in bag)
				{
					if (!CheckConclusionsValidity(solution, element.Conclusions))
					{
						allConclusionsAreValid = false;
						break;
					}
				}

				if (!@this.CheckConclusionValidityAfterSearched || allConclusionsAreValid)
				{
					foreach (var step in bag)
					{
						if (
							@this.RecordStep(
								steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result,
								cancellationToken)
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
					if (@this.EnableGarbageCollectionForcedly)
					{
						GC.Collect();
					}

					if (progress is not null)
					{
						ReportProgress(cloneation, progress, ref progressResult);
					}

					goto Restart;
				}
				else
				{
					var solutionCopied = solution;
					throw new WrongStepException(grid, bag.First(first));

					bool first(StepInfo step) => !CheckConclusionsValidity(solutionCopied, step.Conclusions);
				}
			}
			else
			{
				// If the searcher is only used in the fast mode, just skip it.
				if (
					(
						@this.OptimizedApplyingOrder
						? from info in bag orderby info.Difficulty select info
						: bag.AsEnumerable()
					).FirstOrDefault() is not { } step
				)
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
						stopwatch.Stop();
						return result;
					}

					// The puzzle has not been finished,
					// we should turn to the first step finder
					// to continue solving puzzle.
					bag.Clear();
					if (@this.EnableGarbageCollectionForcedly)
					{
						GC.Collect();
					}

					if (progress is not null)
					{
						ReportProgress(cloneation, progress, ref progressResult);
					}

					goto Restart;
				}

				throw new WrongStepException(grid, step);
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

		static int cmp(in StepSearcher a, in StepSearcher b) =>
			TechniqueProperties.FromSearcher(a)!.Priority - TechniqueProperties.FromSearcher(b)!.Priority;
	}
}
