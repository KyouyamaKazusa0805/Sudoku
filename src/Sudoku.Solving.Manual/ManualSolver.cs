namespace Sudoku.Solving.Manual;

using static SearcherFailedReason;

/// <summary>
/// Provides a manual solver that solves a sudoku puzzle using the human minds and ways
/// to check and solve a sudoku puzzle.
/// </summary>
public sealed partial class ManualSolver : IComplexSolver<ManualSolver, ManualSolverResult>, IManualSolverOptions
{
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool IsFullApplying { get; set; }

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool IgnoreSlowAlgorithms { get; set; }

	/// <inheritdoc/>
	[DisallowNull]
	public IStepSearcher[]? CustomSearcherCollection { get; set; }

	/// <summary>
	/// Indicates the target step searcher collection.
	/// </summary>
	private IStepSearcher[] TargetSearcherCollection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (
			from searcher in CustomSearcherCollection ?? StepSearcherPool.Collection
			where searcher.Options.EnabledArea.Flags(SearcherEnabledArea.Default)
			select searcher
		).ToArray();
	}


	/// <summary>
	/// Indicates the event that a step or a list of steps is applied.
	/// </summary>
	public event StepAppliedEventHandler? StepApplied;


	/// <inheritdoc/>
	public ManualSolverResult Solve(scoped in Grid puzzle, CancellationToken cancellationToken = default)
	{
		var result = new ManualSolverResult(puzzle);
		if (puzzle.ExactlyValidate(out var solution, out bool? sukaku) && sukaku is { } s)
		{
			try
			{
				return InternalSolve(puzzle, solution, s, result, cancellationToken);
			}
			catch (OperationCanceledException ex) when (ex.CancellationToken != cancellationToken)
			{
				throw;
			}
			catch (Exception ex)
			{
				result = result with { IsSolved = false };
				return ex switch
				{
					NotImplementedException or NotSupportedException
						=> result with { FailedReason = NotImplemented },
					WrongStepException { WrongStep: var ws }
						=> result with { FailedReason = WrongStep, WrongStep = ws, UnhandledException = ex },
					OperationCanceledException
						=> result with { FailedReason = UserCancelled },
					_
						=> result with { FailedReason = ExceptionThrown, UnhandledException = ex }
				};
			}
		}
		else
		{
			return result with { IsSolved = false, FailedReason = PuzzleIsInvalid };
		}
	}

	/// <summary>
	/// The inner solving operation method.
	/// </summary>
	/// <param name="puzzle">The original puzzle to be solved.</param>
	/// <param name="solution">The solution of the puzzle. Some step searchers will use this value.</param>
	/// <param name="isSukaku">A <see cref="bool"/> value indicating whether the puzzle is a sukaku.</param>
	/// <param name="resultBase">The base solver result already included the base information.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The solver result.</returns>
	/// <exception cref="WrongStepException">Throws when found wrong steps to apply.</exception>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	private ManualSolverResult InternalSolve(
		scoped in Grid puzzle,
		scoped in Grid solution,
		bool isSukaku,
		ManualSolverResult resultBase,
		CancellationToken cancellationToken = default)
	{
		var playground = puzzle;
		var recordedSteps = new List<IStep>(100);
		var stepGrids = new List<Grid>(100);
		var stepSearchers = TargetSearcherCollection;

		// Sets the solution grid to some step searchers.
		// Some step searchers will use solution grids to solve a puzzle.
		foreach (var stepSearcher in stepSearchers.OfType<IStepSearcherRequiresSolution>())
		{
			stepSearcher.Solution = solution;
		}

		scoped var stopwatch = ValueStopwatch.StartNew();

	TryAgain:
		InitializeMaps(playground);
		foreach (var searcher in stepSearchers)
		{
			switch (isSukaku, searcher, this)
			{
				case (true, { IsNotSupportedForSukaku: true }, _):
				case (_, { Options.EnabledArea: SearcherEnabledArea.None }, _):
				case (_, { IsConfiguredSlow: true }, { IgnoreSlowAlgorithms: true }):
				{
					// Skips on those two cases:
					// 1. Sukaku puzzles can't use techniques that is marked as "not supported for sukaku".
					// 2. If the searcher is currently disabled, just skip it.
					// 3. If the searcher is configured as slow.
					continue;
				}
				case (_, not IBruteForceStepSearcher, { IsFullApplying: true }):
				{
					var accumulator = new List<IStep>();
					searcher.GetAll(accumulator, playground, false);
					if (accumulator.Count == 0)
					{
						continue;
					}

					int appliedStepsCount = 0;
					foreach (var foundStep in accumulator)
					{
						if (verifyConclusionValidity(solution, foundStep))
						{
							bool flag;
							if (flag = recordStep(
									recordedSteps, foundStep, ref playground, ref stopwatch, stepGrids,
									resultBase, cancellationToken, out var result))
							{
								return result!;
							}

							if (flag)
							{
								appliedStepsCount++;
							}
						}
						else
						{
							throw new WrongStepException(playground, foundStep);
						}
					}

					if (appliedStepsCount != 0)
					{
						// Now triggers the event.
						StepApplied?.Invoke(this, new() { Steps = accumulator.ToArray() });
					}

					// The puzzle has not been finished, we should turn to the first step finder
					// to continue solving puzzle.
					goto TryAgain;
				}
				case (_, _, _) when searcher.GetAll(null!, playground, true) is var foundStep:
				{
					switch (foundStep)
					{
						case null:
						case IInvalidStep when ReferenceEquals(IInvalidStep.Instance, foundStep):
						{
							continue;
						}
						default:
						{
							if (verifyConclusionValidity(solution, foundStep))
							{
								bool flag;
								if ((
									flag = recordStep(
										recordedSteps, foundStep, ref playground, ref stopwatch, stepGrids,
										resultBase, cancellationToken, out var result)
									) && flag)
								{
									return result!;
								}

								if (flag)
								{
									// Now triggers the event.
									StepApplied?.Invoke(this, new() { Step = foundStep });
								}
							}
							else
							{
								throw new WrongStepException(playground, foundStep);
							}

							// The puzzle has not been finished, we should turn to the first step finder
							// to continue solving puzzle.
							goto TryAgain;
						}
					}
				}
			}
		}

		// All solver can't finish the puzzle...
		// :(

		return resultBase with
		{
			IsSolved = false,
			FailedReason = PuzzleIsTooHard,
			ElapsedTime = stopwatch.GetElapsedTime(),
			Steps = ImmutableArray.CreateRange(recordedSteps),
			StepGrids = ImmutableArray.CreateRange(stepGrids)
		};


		static bool recordStep(
			ICollection<IStep> steps,
			IStep step,
			scoped ref Grid playground,
			scoped ref ValueStopwatch stopwatch,
			ICollection<Grid> stepGrids,
			ManualSolverResult resultBase,
			CancellationToken cancellationToken,
			[NotNullWhen(true)] out ManualSolverResult? result)
		{
			bool atLeastOneConclusionIsWorth = false;
			foreach (var (t, c, d) in step.Conclusions)
			{
				switch (t)
				{
					case Assignment when playground.GetStatus(c) == CellStatus.Empty:
					case Elimination when playground.Exists(c, d) is true:
					{
						atLeastOneConclusionIsWorth = true;

						goto FinalCheck;
					}
				}
			}

		FinalCheck:
			if (atLeastOneConclusionIsWorth)
			{
				stepGrids.Add(playground);
				step.ApplyTo(ref playground);
				steps.Add(step);

				if (playground.IsSolved)
				{
					result = resultBase with
					{
						IsSolved = true,
						ElapsedTime = stopwatch.GetElapsedTime(),
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

		static bool verifyConclusionValidity(scoped in Grid solution, IStep step)
		{
			foreach (var (t, c, d) in step.Conclusions)
			{
				int digit = solution[c];
				if (t == Assignment && digit != d || t == Elimination && digit == d)
				{
					return false;
				}
			}

			return true;
		}
	}
}
