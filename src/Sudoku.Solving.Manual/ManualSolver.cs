namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides a manual solver that solves a sudoku puzzle using the human minds and ways
/// to check and solve a sudoku puzzle.
/// </summary>
public sealed class ManualSolver : IComplexSolver<ManualSolverResult>, IManualSolverOptions
{
	/// <inheritdoc/>
	public bool IsHodokuMode { get; set; } = true;

	/// <inheritdoc/>
	public bool IsFastSearching { get; set; }

	/// <inheritdoc/>
	public bool OptimizedApplyingOrder { get; set; }


	/// <inheritdoc/>
	public ManualSolverResult Solve(in Grid puzzle, CancellationToken cancellationToken = default)
	{
		var solverResult = new ManualSolverResult(puzzle);
		if (puzzle.ExactlyValidate(out var solution, out bool? sukaku))
		{
			try
			{
				return IsHodokuMode
					// Hodoku mode.
					? Solve_HodokuMode(puzzle, solution, sukaku.Value, solverResult, cancellationToken)
					// Sudoku explainer mode.
					// TODO: Implement a sudoku-explainer mode solving module.
					: throw new NotImplementedException();
			}
			catch (NotImplementedException)
			{
				return solverResult with { IsSolved = false, FailedReason = FailedReason.NotImplemented };
			}
			catch (WrongStepException ex)
			{
				return solverResult with
				{
					IsSolved = false,
					FailedReason = FailedReason.WrongStep,
					WrongStep = ex.WrongStep
				};
			}
			catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
			{
				return solverResult with { IsSolved = false, FailedReason = FailedReason.UserCancelled };
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				return solverResult with
				{
					IsSolved = false,
					FailedReason = FailedReason.ExceptionThrown,
					UnhandledException = ex
				};
			}
		}
		else
		{
			return solverResult with { IsSolved = false, FailedReason = FailedReason.PuzzleIsInvalid };
		}
	}


	/// <summary>
	/// The inner solving operation method.
	/// </summary>
	/// <param name="puzzle">The original puzzle to be solved.</param>
	/// <param name="solution">The solution of the puzzle. Some step searchers will use this value.</param>
	/// <param name="isSukaku">A <see cref="bool"/> value indicating whether the puzzle is a sukaku.</param>
	/// <param name="baseSolverResult">The base solver result already included the base information.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The solver result.</returns>
	/// <exception cref="WrongStepException">Throws when found wrong steps to apply.</exception>
	/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
	private ManualSolverResult Solve_HodokuMode(
		in Grid puzzle, in Grid solution, bool isSukaku, ManualSolverResult baseSolverResult,
		CancellationToken cancellationToken = default)
	{
		var playground = puzzle;
		List<Step> tempSteps = new(20), recordedSteps = new(100);
		var stepGrids = new List<Grid>(100);

		var stepSearchers = StepSearcherPool.Collection;

		// Bug fix: Sets the solution grid to the step searchers that will use the property value.
		((BruteForceStepSearcher)StepSearcherPool.Collection[^1]).Solution = solution;

		var stopwatch = new Stopwatch();
		stopwatch.Start();

	Restart:
		tempSteps.Clear();

		InitializeMaps(playground);
		for (int i = 0, length = stepSearchers.Length; i < length; i++)
		{
			var searcher = stepSearchers[i];
			if (isSukaku && searcher is IDeadlyPatternStepSearcher || searcher.Options.EnabledArea == EnabledArea.None)
			{
				// Skips on those two cases:
				// 1. Sukaku puzzles can't use deadly pattern techniques.
				// 2. If the searcher is currently disabled, just skip it.
				continue;
			}

			searcher.GetAll(tempSteps, playground, false);
			if (tempSteps.Count == 0)
			{
				// Nothing found.
				continue;
			}

			if (IsFastSearching)
			{
				Step? wrongStep = null;
				foreach (var tempStep in tempSteps)
				{
					if (!AreConclusionsValid(solution, tempStep))
					{
						wrongStep = tempStep;
						break;
					}
				}

				if (wrongStep is null)
				{
					foreach (var step in tempSteps)
					{
						if (
							RecordStep(
								recordedSteps, step, ref playground, stopwatch, stepGrids,
								baseSolverResult, cancellationToken, out var result)
						)
						{
							return result;
						}
					}

					// The puzzle has not been finished, we should turn to the first step finder
					// to continue solving puzzle.
					goto Restart;
				}

				throw new WrongStepException(puzzle, wrongStep);
			}
			else
			{
				// If the searcher is only used in the fast mode, just skip it.
				if (
					Enumerable.FirstOrDefault<Step>(
						OptimizedApplyingOrder
							? from info in tempSteps orderby info.Difficulty select info
							: tempSteps
					) is not { } step
				)
				{
					// If current step can't find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (AreConclusionsValid(solution, step))
				{
					if (
						RecordStep(
							recordedSteps, step, ref playground, stopwatch, stepGrids,
							baseSolverResult, cancellationToken, out var result)
					)
					{
						return result;
					}

					// The puzzle has not been finished, we should turn to the first step finder
					// to continue solving puzzle.
					goto Restart;
				}

				throw new WrongStepException(puzzle, step);
			}
		}

		// All solver can't finish the puzzle...
		// :(
		stopwatch.Stop();
		return baseSolverResult with
		{
			IsSolved = false,
			FailedReason = FailedReason.PuzzleIsTooHard,
			ElapsedTime = stopwatch.Elapsed,
			Steps = ImmutableArray.CreateRange(recordedSteps),
			StepGrids = ImmutableArray.CreateRange(stepGrids)
		};
	}

	/// <summary>
	/// <para>
	/// Records the current found and valid step into the specified collection. This method will also
	/// check the validity of <paramref name="cancellationToken"/>. If user has cancelled the operation,
	/// here we'll throw an exception and exit the operation directly.
	/// </para>
	/// <para>
	/// Please note that if the argument <paramref name="result"/> isn't <see langword="null"/>, it'll mean
	/// that the puzzle has been already solved, so this method will stop the stopwatch. Therefore, you don't
	/// need to stop that stopwatch manually like the code <c>stopwatch.Stop();</c>.
	/// </para>
	/// </summary>
	/// <param name="steps">The steps.</param>
	/// <param name="step">The step.</param>
	/// <param name="playground">The playground.</param>
	/// <param name="stopwatch">The stopwatch.</param>
	/// <param name="stepGrids">The step grids.</param>
	/// <param name="baseSolverResult">Indicates the base solver result.</param>
	/// <param name="cancellationToken">The cancellation token that is used to cancel the operation.</param>
	/// <param name="result">The analysis result.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="OperationCanceledException">
	/// Throws when the current operation is cancelled.
	/// </exception>
	private bool RecordStep(
		ICollection<Step> steps, Step step, ref Grid playground, Stopwatch stopwatch,
		ICollection<Grid> stepGrids, ManualSolverResult baseSolverResult,
		CancellationToken cancellationToken, [NotNullWhen(true)] out ManualSolverResult? result)
	{
		bool atLeastOneStepIsWorth = false;
		foreach (var (t, c, d) in step.Conclusions)
		{
			switch (t)
			{
				case ConclusionType.Assignment when playground.GetStatus(c) == CellStatus.Empty:
				case ConclusionType.Elimination when playground.Exists(c, d) is true:
				{
					atLeastOneStepIsWorth = true;

					goto FinalCheck;
				}
			}
		}

	FinalCheck:
		if (atLeastOneStepIsWorth)
		{
			stepGrids.Add(playground);
			step.ApplyTo(ref playground);
			steps.Add(step);

			if (playground.IsSolved)
			{
				stopwatch.Stop();

				result = baseSolverResult with
				{
					IsSolved = true,
					ElapsedTime = stopwatch.Elapsed,
					Steps = ImmutableArray.CreateRange(steps),
					StepGrids = ImmutableArray.CreateRange(stepGrids)
				};
				return true;
			}
		}

		cancellationToken.ThrowIfCancellationRequested();

		result = null;
		return false;
	}


	/// <summary>
	/// Peeks the validity of step, to check whether all conclusions are possibly correct.
	/// </summary>
	/// <param name="solution">The solution.</param>
	/// <param name="step">The step to check.</param>
	/// <returns>A <see cref="bool"/> indicating that.</returns>
	private static bool AreConclusionsValid(in Grid solution, Step step)
	{
		foreach (var (t, c, d) in step.Conclusions)
		{
			int digit = solution[c];
			if (t == ConclusionType.Assignment && digit != d
				|| t == ConclusionType.Elimination && digit == d)
			{
				return false;
			}
		}

		return true;
	}
}
