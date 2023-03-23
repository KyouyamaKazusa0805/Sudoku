namespace Sudoku.Analytics;

/// <summary>
/// Provides an analyzer that solves a sudoku puzzle using the human-friendly logics, and creates an <see cref="AnalyzerResult"/> instance
/// indicating the analytics data.
/// </summary>
/// <seealso cref="AnalyzerResult"/>
public sealed class Analyzer : IAnalyzer<Analyzer, AnalyzerResult>
{
	/// <summary>
	/// Indicates whether the solver will apply all found steps in a step searcher,
	/// in order to solve a puzzle faster. If the value is <see langword="true"/>,
	/// the third argument of <see cref="StepSearcher.GetAll(ref AnalysisContext)"/>
	/// will be set <see langword="false"/> value, in order to find all possible steps in a step searcher,
	/// and all steps will be applied at the same time.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcher.GetAll(ref AnalysisContext)"/>
	public bool IsFullApplying { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="ConditionalCase.UnlimitedTimeComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="ConditionalCase.UnlimitedTimeComplexity"/>
	public bool IgnoreSlowAlgorithms { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured
	/// <see cref="ConditionalCase.UnlimitedSpaceComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="ConditionalCase.UnlimitedSpaceComplexity"/>
	public bool IgnoreHighAllocationAlgorithms { get; set; }

	/// <summary>
	/// <para>
	/// Indicates the custom <see cref="StepSearcher"/>s you defined to solve a puzzle. By default,
	/// the solver will use <see cref="StepSearcherPool.BuiltIn"/> to solve a puzzle.
	/// If you assign a new array of <see cref="StepSearcher"/>s into this property
	/// the step searchers will use this property instead of <see cref="StepSearcherPool.BuiltIn"/> to solve a puzzle.
	/// </para>
	/// <para>
	/// Please note that the property will keep the <see langword="null"/> value if you don't assign any values into it;
	/// however, if you want to use the customized collection to solve a puzzle, assign a non-<see langword="null"/>
	/// array into it.
	/// </para>
	/// </summary>
	/// <seealso cref="StepSearcherPool.BuiltIn"/>
	[DisallowNull]
	public StepSearcher[]? CustomStepSearchers { get; set; }


	/// <inheritdoc/>
	public AnalyzerResult Analyze(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		var result = new AnalyzerResult(puzzle);
		if (puzzle.ExactlyValidate(out var solution, out var sukaku) && sukaku is { } isSukaku)
		{
			try
			{
				return InternalSolve(puzzle, solution, isSukaku, result, progress, cancellationToken);
			}
			catch (OperationCanceledException ex) when (ex.CancellationToken != cancellationToken)
			{
				throw;
			}
			catch (Exception ex)
			{
				return ex switch
				{
					NotImplementedException or NotSupportedException
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.NotImplemented },
					WrongStepException { WrongStep: var s }
						=> result with
						{
							IsSolved = false,
							FailedReason = AnalyzerFailedReason.WrongStep,
							WrongStep = s,
							UnhandledException = ex
						},
					OperationCanceledException
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.UserCancelled },
					_
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.ExceptionThrown, UnhandledException = ex }
				};
			}
		}
		else
		{
			return result with { IsSolved = false, FailedReason = AnalyzerFailedReason.PuzzleIsInvalid };
		}
	}

	/// <summary>
	/// The inner solving operation method.
	/// </summary>
	/// <param name="puzzle">
	/// <inheritdoc cref="Analyze(in Grid, IProgress{double}?, CancellationToken)" path="/param[@name='puzzle']"/>
	/// </param>
	/// <param name="solution">The solution of the puzzle. Some step searchers will use this value.</param>
	/// <param name="isSukaku">A <see cref="bool"/> value indicating whether the puzzle is a sukaku.</param>
	/// <param name="resultBase">The base solver result already included the base information.</param>
	/// <param name="progress">
	/// <inheritdoc cref="Analyze(in Grid, IProgress{double}?, CancellationToken)" path="/param[@name='progress']"/>
	/// </param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Analyze(in Grid, IProgress{double}?, CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns>The solver result.</returns>
	/// <exception cref="WrongStepException">Throws when found wrong steps to apply.</exception>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	private AnalyzerResult InternalSolve(
		scoped in Grid puzzle,
		scoped in Grid solution,
		bool isSukaku,
		AnalyzerResult resultBase,
		IProgress<double>? progress = null,
		CancellationToken cancellationToken = default
	)
	{
		var playground = puzzle;
		var totalCandidatesCount = playground.CandidatesCount;
		var recordedSteps = new List<Step>(100);
		var stepGrids = new List<Grid>(100);
		var stepSearchers = (
			from searcher in CustomStepSearchers ?? StepSearcherPool.BuiltIn
			where searcher.RunningArea.Flags(StepSearcherRunningArea.Searching)
			select searcher
		).ToArray();

		scoped var stopwatch = ValueStopwatch.StartNew();

	Again:
		Initialize(playground, solution);
		foreach (var searcher in stepSearchers)
		{
			switch (isSukaku, searcher, this)
			{
				case (true, { IsNotSupportedForSukaku: true }, _):
				case (_, { RunningArea: StepSearcherRunningArea.None }, _):
				case (_, { IsConfiguredSlow: true }, { IgnoreSlowAlgorithms: true }):
				case (_, { IsConfiguredHighAllocation: true }, { IgnoreHighAllocationAlgorithms: true }):
				{
					// Skips on those two cases:
					// 1. Sukaku puzzles can't use techniques that is marked as "not supported for sukaku".
					// 2. If the searcher is currently disabled, just skip it.
					// 3. If the searcher is configured as slow.
					// 4. If the searcher is configured as high-allocation.
					continue;
				}
				case (_, not BruteForceStepSearcher, { IsFullApplying: true }):
				{
					var accumulator = new List<Step>();
					scoped var context = new AnalysisContext(accumulator, playground, false);
					searcher.GetAll(ref context);
					if (accumulator.Count == 0)
					{
						continue;
					}

					foreach (var foundStep in accumulator)
					{
						if (verifyConclusionValidity(solution, foundStep))
						{
							if (
								RecordStep(
									recordedSteps, foundStep, ref playground, ref stopwatch, stepGrids,
									resultBase, cancellationToken, out var result)
							)
							{
								return result;
							}
						}
						else
						{
							throw new WrongStepException(playground, foundStep);
						}
					}

					// The puzzle has not been finished, we should turn to the first step finder
					// to continue solving puzzle.
					goto ReportStatusAndSkipToTryAgain;
				}
				default:
				{
					scoped var context = new AnalysisContext(null, playground, true);
					switch (searcher.GetAll(ref context))
					{
						case null or InvalidStep:
						{
							continue;
						}
						case var foundStep:
						{
							if (verifyConclusionValidity(solution, foundStep))
							{
								if (
									RecordStep(
										recordedSteps, foundStep, ref playground, ref stopwatch, stepGrids,
										resultBase, cancellationToken, out var result)
								)
								{
									return result;
								}
							}
							else
							{
								throw new WrongStepException(playground, foundStep);
							}

							// The puzzle has not been finished, we should turn to the first step finder
							// to continue solving puzzle.
							goto ReportStatusAndSkipToTryAgain;
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
			FailedReason = AnalyzerFailedReason.PuzzleIsTooHard,
			ElapsedTime = stopwatch.GetElapsedTime(),
			Steps = recordedSteps.ToArray(),
			StepGrids = stepGrids.ToArray()
		};

	ReportStatusAndSkipToTryAgain:
		progress?.Report((double)(totalCandidatesCount - playground.CandidatesCount) / totalCandidatesCount);
		goto Again;


		static bool verifyConclusionValidity(scoped in Grid solution, Step step)
		{
			foreach (var (t, c, d) in step.Conclusions)
			{
				var digit = solution[c];
				if (t == Assignment && digit != d || t == Elimination && digit == d)
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Try to record the current step, saving it into the argument <paramref name="steps"/>.
	/// </summary>
	/// <param name="steps">The accumulator.</param>
	/// <param name="step">The step to be saved.</param>
	/// <param name="playground">The currently used grid.</param>
	/// <param name="stopwatch">The stopwatch, in order to check elapsed time.</param>
	/// <param name="stepGrids">The step grids.</param>
	/// <param name="resultBase">The <see cref="AnalyzerResult"/> instance as a base value provider.</param>
	/// <param name="cancellationToken">The cancellation token that allows user cancelling the operation.</param>
	/// <param name="result">
	/// The result returned.
	/// If this step is the final step, the result <see cref="AnalyzerResult"/> instance will be assigned into this argument.
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the current step is worth to be saved,
	/// meaning it contains at least one conclusion that the current grid <paramref name="playground"/> contains.
	/// </returns>
	private static bool RecordStep(
		List<Step> steps,
		Step step,
		scoped ref Grid playground,
		scoped ref ValueStopwatch stopwatch,
		List<Grid> stepGrids,
		AnalyzerResult resultBase,
		CancellationToken cancellationToken,
		[NotNullWhen(true)] out AnalyzerResult? result
	)
	{
		var atLeastOneConclusionIsWorth = false;
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
					Solution = playground,
					ElapsedTime = stopwatch.GetElapsedTime(),
					Steps = steps.ToArray(),
					StepGrids = stepGrids.ToArray()
				};
				return true;
			}
		}
		else
		{
			// No steps are available.
			goto ReturnFalse;
		}

		cancellationToken.ThrowIfCancellationRequested();

	ReturnFalse:
		result = null;
		return false;
	}
}
