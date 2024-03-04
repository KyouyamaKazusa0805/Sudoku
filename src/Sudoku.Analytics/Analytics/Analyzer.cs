#undef REMOVE_DUPLICATED_STEPS_IN_SINGLES_IF_RANDOM_ENABLED

namespace Sudoku.Analytics;

/// <summary>
/// Provides an analyzer that solves a sudoku puzzle using the human-friendly logics, and creates an <see cref="AnalyzerResult"/> instance
/// indicating the analytics data.
/// </summary>
/// <remarks>
/// Please note that this type has no accessible constructors,
/// you can just use type <see cref="PredefinedAnalyzers"/> to get <see cref="Analyzer"/>s you want to get.
/// In addition, you can also use <see cref="AnalyzerFactory"/> to create some extra configuration.
/// </remarks>
/// <seealso cref="AnalyzerResult"/>
/// <seealso cref="PredefinedAnalyzers"/>
/// <seealso cref="AnalyzerFactory"/>
/// <completionlist cref="PredefinedAnalyzers"/>
public sealed partial class Analyzer :
	AnalyzerOrCollector,
	IRandomizedAnalyzer<Analyzer, AnalyzerResult>,
	ICultureSupportedAnalyzer<Analyzer, AnalyzerResult>
{
	/// <summary>
	/// Indicates the default steps capacity.
	/// </summary>
	private const int DefaultStepsCapacity = 54;


	/// <summary>
	/// The random number generator.
	/// </summary>
	private readonly Random _random = new();


	/// <inheritdoc/>
	public bool RandomizedChoosing { get; set; }

	/// <inheritdoc/>
	public bool IsFullApplying { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="StepSearcherFlags.TimeComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcherFlags.TimeComplexity"/>
	public bool IgnoreSlowAlgorithms { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="StepSearcherFlags.SpaceComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcherFlags.SpaceComplexity"/>
	public bool IgnoreHighAllocationAlgorithms { get; set; }

	/// <inheritdoc/>
	public CultureInfo? CurrentCulture { get; set; }

	/// <inheritdoc/>
	[DisallowNull]
	[ImplicitField(RequiredReadOnlyModifier = false)]
	public override StepSearcher[]? StepSearchers
	{
		get => _stepSearchers;

		set => ResultStepSearchers = FilterStepSearchers(_stepSearchers = value, StepSearcherRunningArea.Searching);
	}

	/// <inheritdoc/>
	public override StepSearcher[] ResultStepSearchers { get; protected internal set; } =
		from searcher in StepSearcherPool.BuiltInStepSearchersExpanded
		where searcher.RunningArea.HasFlag(StepSearcherRunningArea.Searching)
		select searcher;

	/// <inheritdoc/>
	public override StepSearcherOptions Options { get; set; } = StepSearcherOptions.Default;

	/// <inheritdoc/>
	Random IRandomizedAnalyzer<Analyzer, AnalyzerResult>.RandomNumberGenerator => _random;

	/// <summary>
	/// Indicates the final <see cref="CultureInfo"/> instance to be used.
	/// </summary>
	private CultureInfo ResultCurrentCulture => CurrentCulture ?? CultureInfo.CurrentUICulture;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has already been solved.</exception>
	public AnalyzerResult Analyze(scoped ref readonly Grid puzzle, IProgress<AnalyzerProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved)
		{
			throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("GridAlreadySolved"));
		}

		var result = new AnalyzerResult(in puzzle) { IsSolved = false };
		if (puzzle.ExactlyValidate(out var solution, out var sukaku) && sukaku is { } isSukaku)
		{
			// Firstly, we should check whether the puzzle is a GSP.
			puzzle.InferSymmetricalPlacement(out var symmetricType, out var mappingDigits, out var selfPairedDigitsMask);

			try
			{
				return analyzeInternal(
					in puzzle,
					in solution,
					isSukaku,
					result,
					symmetricType,
					mappingDigits,
					selfPairedDigitsMask,
					progress,
					cancellationToken
				);
			}
			catch (Exception ex)
			{
				return ex switch
				{
					RuntimeAnalyticsException e => e switch
					{
						WrongStepException => result with { IsSolved = false, FailedReason = FailedReason.WrongStep, UnhandledException = e },
						PuzzleInvalidException => result with { IsSolved = false, FailedReason = FailedReason.PuzzleIsInvalid }
					},
					OperationCanceledException { CancellationToken: var c } when c == cancellationToken
						=> result with { IsSolved = false, FailedReason = FailedReason.UserCancelled },
					NotImplementedException or NotSupportedException => result with { IsSolved = false, FailedReason = FailedReason.NotImplemented },
					_ => result with { IsSolved = false, FailedReason = FailedReason.ExceptionThrown, UnhandledException = ex }
				};
			}
		}
		return result with { IsSolved = false, FailedReason = FailedReason.PuzzleIsInvalid };


		AnalyzerResult analyzeInternal(
			scoped ref readonly Grid puzzle,
			scoped ref readonly Grid solution,
			bool isSukaku,
			AnalyzerResult resultBase,
			SymmetricType symmetricType,
			scoped ReadOnlySpan<Digit?> mappingDigits,
			Mask selfPairedDigitsMask,
			IProgress<AnalyzerProgress>? progress,
			CancellationToken cancellationToken
		)
		{
			var playground = puzzle;
			var totalCandidatesCount = playground.CandidatesCount;
			var (collectedSteps, stepGrids, stepSearchers) = (new List<Step>(DefaultStepsCapacity), new List<Grid>(DefaultStepsCapacity), ResultStepSearchers);
			scoped var stopwatch = ValueStopwatch.NewInstance;
			var accumulator = IsFullApplying || RandomizedChoosing ? [] : default(List<Step>);
			scoped var context = new AnalysisContext(accumulator, in playground, in puzzle, !IsFullApplying && !RandomizedChoosing, Options);

			// Determine whether the grid is a GSP pattern. If so, check for eliminations.
			if ((symmetricType, selfPairedDigitsMask) is (not SymmetricType.None, not 0) && !mappingDigits.IsEmpty)
			{
				context.GspPatternInferred = symmetricType;
				context.MappingRelations = mappingDigits;

				if (SymmetricalPlacing.GetStep(in playground, Options) is { } step)
				{
					if (verifyConclusionValidity(in solution, step))
					{
						if (onCollectingSteps(
							collectedSteps, step, in context, ref playground, in stopwatch,
							stepGrids, resultBase, cancellationToken, out var result))
						{
							return result;
						}
					}
					else
					{
						throw new WrongStepException(in playground, step);
					}
				}
			}

		FindNextStep:
			Initialize(in playground, in solution);
			string progressedStepSearcherName;
			foreach (var searcher in stepSearchers)
			{
				switch (isSukaku, searcher, this)
				{
					case (true, { Metadata.IsNotSupportedForSukaku: true }, _):
					case (_, { RunningArea: StepSearcherRunningArea.None }, _):
					case (_, { Metadata.IsConfiguredSlow: true }, { IgnoreSlowAlgorithms: true }):
					case (_, { Metadata.IsConfiguredHighAllocation: true }, { IgnoreHighAllocationAlgorithms: true }):
					case (_, { Metadata.IsOnlyRunForDirectViews: true }, { Options: { DistinctDirectMode: true, IsDirectMode: false } }):
					case (_, { Metadata.IsOnlyRunForIndirectViews: true }, { Options: { DistinctDirectMode: true, IsDirectMode: true } }):
					{
						// Skips on those two cases:
						// 1. Sukaku puzzles can't use techniques that is marked as "not supported for sukaku".
						// 2. If the searcher is currently disabled.
						// 3. If the searcher is configured as slow.
						// 4. If the searcher is configured as high-allocation.
						// 5. If the searcher is only run for direct view, and the current is indirect view.
						// 6. If the searcher is only run for indirect view, and the current is direct view.
						continue;
					}
					case (_, BruteForceStepSearcher, { RandomizedChoosing: true }):
					{
						accumulator!.Clear();

						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						// Here will fetch a correct step to be applied.
						var chosenStep = accumulator[_random.Next(0, accumulator.Count)];
						if (!verifyConclusionValidity(in solution, chosenStep))
						{
							throw new WrongStepException(in playground, chosenStep);
						}

						if (onCollectingSteps(
							collectedSteps, chosenStep, in context, ref playground,
							in stopwatch, stepGrids, resultBase, cancellationToken, out var result))
						{
							return result;
						}

						goto MakeProgress;
					}
#if REMOVE_DUPLICATED_STEPS_IN_SINGLES_IF_RANDOM_ENABLED
					case (_, SingleStepSearcher, { RandomizedChoosing: true }):
					{
						// Randomly select a step won't take any effects on single steps.
						accumulator!.Clear();

						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						var temp = new List<Step>();
						if (accumulator.Count == 1)
						{
							temp.Add(accumulator[0]);
						}
						else
						{
							var distinctCandidatesKey = (CandidateMap)[];
							foreach (SingleStep step in accumulator)
							{
								if (!distinctCandidatesKey.Contains(step.Cell * 9 + step.Digit))
								{
									temp.Add(step);
									distinctCandidatesKey.Add(step.Cell * 9 + step.Digit);
								}
							}
						}

						// Here will fetch a correct step to be applied.
						var chosenStep = temp[_random.Next(0, temp.Count)];
						if (!verifyConclusionValidity(in solution, chosenStep))
						{
							throw new WrongStepException(in playground, chosenStep);
						}

						if (onCollectingSteps(
							collectedSteps, chosenStep, in context, ref playground,
							in stopwatch, stepGrids, resultBase, cancellationToken, out var result))
						{
							return result;
						}
						break;
					}
#endif
					case (_, not BruteForceStepSearcher, { IsFullApplying: true } or { RandomizedChoosing: true }):
					{
						accumulator!.Clear();

						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						if (RandomizedChoosing)
						{
							// Here will fetch a correct step to be applied.
							var chosenStep = accumulator[_random.Next(0, accumulator.Count)];
							if (!verifyConclusionValidity(in solution, chosenStep))
							{
								throw new WrongStepException(in playground, chosenStep);
							}

							if (onCollectingSteps(
								collectedSteps, chosenStep, in context, ref playground,
								in stopwatch, stepGrids, resultBase, cancellationToken, out var result))
							{
								return result;
							}
						}
						else
						{
							foreach (var foundStep in accumulator)
							{
								if (!verifyConclusionValidity(in solution, foundStep))
								{
									throw new WrongStepException(in playground, foundStep);
								}

								if (onCollectingSteps(
									collectedSteps, foundStep, in context, ref playground, in stopwatch, stepGrids,
									resultBase, cancellationToken, out var result))
								{
									return result;
								}
							}
						}

						// The puzzle has not been finished, we should turn to the first step finder
						// to continue solving puzzle.
						goto MakeProgress;
					}
					default:
					{
						switch (searcher.Collect(ref context))
						{
							case null:
							{
								continue;
							}
							case var foundStep:
							{
								if (verifyConclusionValidity(in solution, foundStep))
								{
									if (onCollectingSteps(
										collectedSteps, foundStep, in context, ref playground, in stopwatch, stepGrids,
										resultBase, cancellationToken, out var result))
									{
										return result;
									}
								}
								else
								{
									throw new WrongStepException(in playground, foundStep);
								}

								// The puzzle has not been finished, we should turn to the first step finder
								// to continue solving puzzle.
								goto MakeProgress;
							}
						}
					}
				}

			MakeProgress:
				progressedStepSearcherName = searcher.ToString(ResultCurrentCulture);
				goto ReportStateAndTryNextStep;
			}

			// All solver can't finish the puzzle... :(
			return resultBase with
			{
				FailedReason = FailedReason.PuzzleIsTooHard,
				ElapsedTime = stopwatch.ElapsedTime,
				Steps = [.. collectedSteps],
				SteppingGrids = [.. stepGrids]
			};

		ReportStateAndTryNextStep:
			progress?.Report(new(progressedStepSearcherName, (double)(totalCandidatesCount - playground.CandidatesCount) / totalCandidatesCount));
			goto FindNextStep;


			static bool verifyConclusionValidity(scoped ref readonly Grid solution, Step step)
			{
				foreach (var (t, c, d) in step.Conclusions)
				{
					var digit = solution.GetDigit(c);
					if (t == Assignment && digit != d || t == Elimination && digit == d)
					{
						return false;
					}
				}

				return true;
			}

			static bool onCollectingSteps(
				List<Step> steps,
				Step step,
				scoped ref readonly AnalysisContext context,
				scoped ref Grid playground,
				scoped ref readonly ValueStopwatch stopwatch,
				List<Grid> steppingGrids,
				AnalyzerResult resultBase,
				CancellationToken cancellationToken,
				[NotNullWhen(true)] out AnalyzerResult? result
			)
			{
				// Optimization: If the grid is inferred as a GSP pattern, we can directly add extra eliminations at symmetric positions.
				if (context is { GspPatternInferred: { } symmetricType } && step is not GurthSymmetricalPlacementStep)
				{
					scoped var mappingRelations = context.MappingRelations;
					var originalConclusions = step.Conclusions;
					var newConclusions = new List<Conclusion>();
					foreach (var conclusion in originalConclusions)
					{
						var newConclusion = conclusion.GetSymmetricConclusion(symmetricType, mappingRelations[conclusion.Digit] ?? -1);
						if (newConclusion != conclusion && playground.Exists(newConclusion.Cell, newConclusion.Digit) is true)
						{
							newConclusions.Add(newConclusion);
						}
					}

					step.Conclusions = [.. originalConclusions, .. newConclusions];
				}

				var atLeastOneConclusionIsWorth = false;
				foreach (var (t, c, d) in step.Conclusions)
				{
					switch (t)
					{
						case Assignment when playground.GetState(c) == CellState.Empty:
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
					steppingGrids.Add(playground);
					playground.Apply(step);
					steps.Add(step);

					if (playground.IsSolved)
					{
						result = resultBase with
						{
							IsSolved = true,
							Solution = playground,
							ElapsedTime = stopwatch.ElapsedTime,
							Steps = [.. steps],
							SteppingGrids = [.. steppingGrids]
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
	}
}
