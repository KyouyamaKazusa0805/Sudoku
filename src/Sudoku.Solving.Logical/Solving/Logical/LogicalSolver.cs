namespace Sudoku.Solving.Logical;

using static SearcherFailedReason;

/// <summary>
/// Provides a solver that solves a sudoku puzzle using the human-friendly logics.
/// </summary>
/// <remarks>
/// Note: if you want to create an instance, I recommend you use another type called <see cref="CommonLogicalSolvers"/>
/// to get your desired instance. Two commonly-used instances of that type are:
/// <list type="table">
/// <listheader>
/// <term>Instance</term>
/// <description>Description</description>
/// </listheader>
/// <item>
/// <term><see cref="CommonLogicalSolvers.Default"/></term>
/// <description>A default solver that uses parameterless constructor to instantiate, without any extra properties assigned.</description>
/// </item>
/// <item>
/// <term><see cref="CommonLogicalSolvers.Suitable"/></term>
/// <description>A powerful instance that has already configured some properties.</description>
/// </item>
/// </list>
/// If you want to change some properties in this type, you can use C# 9 syntax "<see langword="with"/> Expressions in Records"
/// to describe the new assigning logic, e.g.
/// <code><![CDATA[
/// var yourOwnSolver = CommonLogicalSolvers.Suitable with
/// {
///     IsFullApplying = true,
///     IgnoreSlowAlgorithms = false
/// };
/// ]]></code>
/// </remarks>
/// <completionlist cref="CommonLogicalSolvers"/>
public sealed partial record LogicalSolver : IComplexSolver<LogicalSolver, LogicalSolverResult>
{
	/// <summary>
	/// Indicates whether the solver will apply all found steps in a step searcher,
	/// in order to solve a puzzle faster. If the value is <see langword="true"/>,
	/// the third argument of <see cref="IStepSearcher.GetAll(in LogicalAnalysisContext)"/>
	/// will be set <see langword="false"/> value, in order to find all possible steps in a step searcher,
	/// and all steps will be applied at the same time.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="IStepSearcher.GetAll(in LogicalAnalysisContext)"/>
	public bool IsFullApplying { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="StepSearcherRunningOptions.SlowAlgorithm"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcherRunningOptions.SlowAlgorithm"/>
	public bool IgnoreSlowAlgorithms { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured
	/// <see cref="StepSearcherRunningOptions.HighMemoryAllocation"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcherRunningOptions.HighMemoryAllocation"/>
	public bool IgnoreHighAllocationAlgorithms { get; set; }

	/// <summary>
	/// <para>
	/// Indicates the custom searcher collection you defined to solve a puzzle. By default,
	/// the solver will use <see cref="StepSearcherPool.Collection"/> to solve a puzzle.
	/// If you assign a new array of <see cref="IStepSearcher"/>s into this property
	/// the step searchers will use this property instead of <see cref="StepSearcherPool.Collection"/> to solve a puzzle.
	/// </para>
	/// <para>
	/// Please note that the property will keep the <see langword="null"/> value if you don't assign any values into it;
	/// however, if you want to use the customized collection to solve a puzzle, assign a non-<see langword="null"/>
	/// array into it.
	/// </para>
	/// </summary>
	/// <seealso cref="StepSearcherPool.Collection"/>
	[DisallowNull]
	public StepSearcherCollection? CustomSearcherCollection { get; set; }

	/// <summary>
	/// Indicates the target step searcher collection.
	/// </summary>
	private StepSearcherCollection TargetSearcherCollection
		=> (
			from searcher in CustomSearcherCollection ?? StepSearcherPool.Collection
			where searcher.Options.EnabledArea.Flags(SearcherEnabledArea.Default)
			select searcher
		).ToArray();


	/// <inheritdoc/>
	public LogicalSolverResult Solve(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		var result = new LogicalSolverResult(puzzle);
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
						=> result with { IsSolved = false, FailedReason = NotImplemented },
					WrongStepException { WrongStep: var s }
						=> result with { IsSolved = false, FailedReason = WrongStep, WrongStep = s, UnhandledException = ex },
					OperationCanceledException
						=> result with { IsSolved = false, FailedReason = UserCancelled },
					_
						=> result with { IsSolved = false, FailedReason = ExceptionThrown, UnhandledException = ex }
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
	/// <param name="puzzle">
	/// <inheritdoc cref="Solve(in Grid, IProgress{double}?, CancellationToken)" path="/param[@name='puzzle']"/>
	/// </param>
	/// <param name="solution">The solution of the puzzle. Some step searchers will use this value.</param>
	/// <param name="isSukaku">A <see cref="bool"/> value indicating whether the puzzle is a sukaku.</param>
	/// <param name="resultBase">The base solver result already included the base information.</param>
	/// <param name="progress">
	/// <inheritdoc cref="Solve(in Grid, IProgress{double}?, CancellationToken)" path="/param[@name='progress']"/>
	/// </param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Solve(in Grid, IProgress{double}?, CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns>The solver result.</returns>
	/// <exception cref="WrongStepException">Throws when found wrong steps to apply.</exception>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	private unsafe LogicalSolverResult InternalSolve(
		scoped in Grid puzzle,
		scoped in Grid solution,
		bool isSukaku,
		LogicalSolverResult resultBase,
		IProgress<double>? progress = null,
		CancellationToken cancellationToken = default
	)
	{
		var playground = puzzle;

		var totalCandidatesCount = playground.CandidatesCount;
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
				case (_, { IsConfiguredHighAllocation: true }, { IgnoreHighAllocationAlgorithms: true }):
				{
					// Skips on those two cases:
					// 1. Sukaku puzzles can't use techniques that is marked as "not supported for sukaku".
					// 2. If the searcher is currently disabled, just skip it.
					// 3. If the searcher is configured as slow.
					// 4. If the searcher is configured as high-allocation.
					continue;
				}
				case (_, not IBruteForceStepSearcher, { IsFullApplying: true }):
				{
					var accumulator = new List<IStep>();
					scoped var context = new LogicalAnalysisContext(accumulator, playground, false);
					searcher.GetAll(context);
					if (accumulator.Count == 0)
					{
						continue;
					}

					foreach (var foundStep in accumulator)
					{
						if (verifyConclusionValidity(solution, foundStep))
						{
							if (
								recordStep(
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
					switch (searcher.GetAll(new(null, playground, true)))
					{
						case null:
						case IInvalidStep foundStep when ReferenceEquals(IInvalidStep.Instance, foundStep):
						{
							continue;
						}
						case var foundStep:
						{
							if (verifyConclusionValidity(solution, foundStep))
							{
								if (
									recordStep(
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
			FailedReason = PuzzleIsTooHard,
			ElapsedTime = stopwatch.GetElapsedTime(),
			Steps = ImmutableArray.CreateRange(recordedSteps),
			StepGrids = ImmutableArray.CreateRange(stepGrids)
		};

	ReportStatusAndSkipToTryAgain:
		progress?.Report((double)(totalCandidatesCount - playground.CandidatesCount) / totalCandidatesCount);
		goto TryAgain;


		static bool recordStep(
			ICollection<IStep> steps,
			IStep step,
			scoped ref Grid playground,
			scoped ref ValueStopwatch stopwatch,
			ICollection<Grid> stepGrids,
			LogicalSolverResult resultBase,
			CancellationToken cancellationToken,
			[NotNullWhen(true)] out LogicalSolverResult? result)
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
						Steps = ImmutableArray.CreateRange(steps),
						StepGrids = ImmutableArray.CreateRange(stepGrids)
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

		static bool verifyConclusionValidity(scoped in Grid solution, IStep step)
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
}
