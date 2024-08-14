#undef REMOVE_DUPLICATED_STEPS_IN_SINGLES_IF_RANDOM_ENABLED

namespace Sudoku.Analytics;

/// <summary>
/// Provides an analyzer that solves a sudoku puzzle using the human-friendly logics,
/// and creates an <see cref="AnalysisResult"/> instance indicating the analytics data.
/// </summary>
/// <seealso cref="AnalysisResult"/>
/// <seealso cref="Analyzer"/>
/// <seealso cref="AnalyzerFactory"/>
public sealed partial class Analyzer : AnalyzerOrCollector, IAnalyzer<Analyzer, AnalyzerContext, AnalysisResult>
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
	[FactoryProperty]
	public bool RandomizedChoosing { get; set; }

	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "ApplyAll", ParameterName = "applyAll")]
	public bool IsFullApplying { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="StepSearcherRuntimeFlags.TimeComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcherRuntimeFlags.TimeComplexity"/>
	[FactoryProperty(MethodSuffixName = "IgnoreHighTimeComplexityStepSearchers", ParameterName = "ignore")]
	public bool IgnoreSlowAlgorithms { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="StepSearcherRuntimeFlags.SpaceComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcherRuntimeFlags.SpaceComplexity"/>
	[FactoryProperty(MethodSuffixName = "IgnoreHighSpaceComplexityStepSearchers", ParameterName = "ignore")]
	public bool IgnoreHighAllocationAlgorithms { get; set; }

	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "Culture", ParameterName = "culture")]
	public IFormatProvider? CurrentCulture { get; set; }

	/// <inheritdoc/>
	[ImplicitField(RequiredReadOnlyModifier = false)]
	[FactoryProperty(ParameterType = typeof(StepSearcher[]), ParameterModifiers = "params")]
	public override ReadOnlyMemory<StepSearcher> StepSearchers
	{
		get => _stepSearchers;

		set => ResultStepSearchers = FilterStepSearchers(_stepSearchers = value, StepSearcherRunningArea.Searching);
	}

	/// <inheritdoc/>
	public override ReadOnlyMemory<StepSearcher> ResultStepSearchers { get; protected internal set; } =
		from searcher in StepSearcherPool.StepSearchers
		where searcher.RunningArea.HasFlag(StepSearcherRunningArea.Searching)
		select searcher;

	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "UserDefinedOptions")]
	public override StepSearcherOptions Options { get; set; } = StepSearcherOptions.Default;

	/// <summary>
	/// Indicates the conditional options to be set.
	/// </summary>
	[FactoryProperty]
	internal StepSearcherConditionalOptions? ConditionalOptions { get; set; } = StepSearcherConditionalOptions.Default;

	/// <inheritdoc/>
	Random IAnalyzer<Analyzer, AnalyzerContext, AnalysisResult>.RandomNumberGenerator => _random;

	/// <summary>
	/// Indicates the final <see cref="CultureInfo"/> instance to be used.
	/// </summary>
	private CultureInfo ResultCurrentCulture => CurrentCulture as CultureInfo ?? CultureInfo.CurrentUICulture;


	/// <summary>
	/// Indicates the default <see cref="Analyzer"/> instance that has no extra configuration.
	/// </summary>
	public static Analyzer Default => new();

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that all possible <see cref="StepSearcher"/> instances are included.
	/// </summary>
	public static Analyzer AllIn
		=> Balanced
			.WithIgnoreHighTimeComplexityStepSearchers(false)
			.WithIgnoreHighSpaceComplexityStepSearchers(false)
			.WithStepSearcherSetters<NormalFishStepSearcher>(static s => { s.DisableFinnedOrSashimiXWing = false; s.AllowSiamese = true; })
			.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 9)
			.WithStepSearcherSetters<ReverseBivalueUniversalGraveStepSearcher>(static s => { s.MaxSearchingEmptyCellsCount = 4; s.AllowPartiallyUsedTypes = true; })
			.WithStepSearcherSetters<ComplexFishStepSearcher>(static s => { s.MaxSize = 7; s.AllowSiamese = true; })
			.WithStepSearcherSetters<XyzRingStepSearcher>(static s => s.AllowSiamese = false)
			.WithStepSearcherSetters<BowmanBingoStepSearcher>(static s => s.MaxLength = 64)
			.WithStepSearcherSetters<AlignedExclusionStepSearcher>(static s => s.MaxSearchingSize = 5);

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that has some extra configuration, suitable for a whole analysis lifecycle.
	/// </summary>
	public static Analyzer Balanced
		=> Default
			.WithIgnoreHighTimeComplexityStepSearchers(false)
			.WithIgnoreHighSpaceComplexityStepSearchers(true)
			.WithStepSearcherSetters<SingleStepSearcher>(static s => { s.EnableFullHouse = true; s.EnableLastDigit = true; s.HiddenSinglesInBlockFirst = true; s.UseIttoryuMode = false; })
			.WithStepSearcherSetters<NormalFishStepSearcher>(static s => { s.DisableFinnedOrSashimiXWing = false; s.AllowSiamese = false; })
			.WithStepSearcherSetters<UniqueRectangleStepSearcher>(static s => { s.AllowIncompleteUniqueRectangles = true; s.SearchForExtendedUniqueRectangles = true; })
			.WithStepSearcherSetters<BivalueUniversalGraveStepSearcher>(static s => s.SearchExtendedTypes = true)
			.WithStepSearcherSetters<ReverseBivalueUniversalGraveStepSearcher>(static s => { s.MaxSearchingEmptyCellsCount = 2; s.AllowPartiallyUsedTypes = true; })
			.WithStepSearcherSetters<AlmostLockedSetsXzStepSearcher>(static s => { s.AllowCollision = true; s.AllowLoopedPatterns = true; })
			.WithStepSearcherSetters<AlmostLockedSetsXyWingStepSearcher>(static s => s.AllowCollision = true)
			.WithStepSearcherSetters<RegularWingStepSearcher>(static s => s.MaxSearchingPivotsCount = 5)
			.WithStepSearcherSetters<TemplateStepSearcher>(static s => s.TemplateDeleteOnly = false)
			.WithStepSearcherSetters<ComplexFishStepSearcher>(static s => { s.MaxSize = 5; s.AllowSiamese = false; })
			.WithStepSearcherSetters<XyzRingStepSearcher>(static s => s.AllowSiamese = false)
			.WithStepSearcherSetters<BowmanBingoStepSearcher>(static s => s.MaxLength = 64)
			.WithStepSearcherSetters<AlmostLockedCandidatesStepSearcher>(static s => s.CheckAlmostLockedQuadruple = false)
			.WithStepSearcherSetters<AlignedExclusionStepSearcher>(static s => s.MaxSearchingSize = 3);

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that only contains SSTS techniques:
	/// <list type="bullet">
	/// <item><see cref="SingleStepSearcher"/></item>
	/// <item><see cref="LockedCandidatesStepSearcher"/></item>
	/// <item><see cref="LockedSubsetStepSearcher"/></item>
	/// <item><see cref="NormalSubsetStepSearcher"/></item>
	/// </list>
	/// </summary>
	/// <seealso cref="SingleStepSearcher"/>
	/// <seealso cref="LockedCandidatesStepSearcher"/>
	/// <seealso cref="LockedSubsetStepSearcher"/>
	/// <seealso cref="NormalSubsetStepSearcher"/>
	public static Analyzer SstsOnly
		=> Default
			.WithStepSearchers(
				new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true, HiddenSinglesInBlockFirst = true, UseIttoryuMode = false },
				new LockedSubsetStepSearcher(),
				new LockedCandidatesStepSearcher(),
				new NormalSubsetStepSearcher()
			)
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });

	/// <summary>
	/// Indicates an <see cref="Analyzer"/> instance that only supports for techniques used in Sudoku Explainer.
	/// </summary>
	public static Analyzer SudokuExplainer
		=> Default
			.WithIgnoreHighTimeComplexityStepSearchers(false)
			.WithIgnoreHighSpaceComplexityStepSearchers(false)
			.WithStepSearchers(
				new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true, HiddenSinglesInBlockFirst = true, UseIttoryuMode = false },
				new LockedSubsetStepSearcher(),
				new LockedCandidatesStepSearcher(),
				new NormalSubsetStepSearcher(),
				new NormalFishStepSearcher { AllowSiamese = false },
				new RegularWingStepSearcher { MaxSearchingPivotsCount = 3 },
				new UniqueRectangleStepSearcher { AllowIncompleteUniqueRectangles = false, SearchForExtendedUniqueRectangles = false },
				new UniqueLoopStepSearcher(),
				new BivalueUniversalGraveStepSearcher { SearchExtendedTypes = false },
				new AlignedExclusionStepSearcher { MaxSearchingSize = 3 },
				new ChainStepSearcher()
			)
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });


	/// <inheritdoc/>
	public AnalysisResult Analyze(ref readonly AnalyzerContext context)
	{
		ref readonly var puzzle = ref context.Puzzle;
		var progress = context.ProgressReporter;
		var cancellationToken = context.CancellationToken;

		if (puzzle.IsSolved)
		{
			throw new InvalidOperationException(SR.ExceptionMessage("GridAlreadySolved"));
		}

		var result = new AnalysisResult(in puzzle) { IsSolved = false };
		var solution = puzzle.GetSolutionGrid();
		if (puzzle.GetUniqueness() != Uniqueness.Bad)
		{
			// We should check whether the puzzle is a GSP firstly.
			// This method doesn't check for Sukaku puzzles, or ones containing multiple solutions.
			SymmetryInferrer.TryInfer(in puzzle, out var triplet);
			var (symmetricType, mappingDigits, selfPairedDigitsMask) = triplet;

			try
			{
				// Here 'puzzle' may contains multiple solutions, so 'solution' may equal to 'Grid.Undefined'.
				// We will defer the checking inside this method stackframe.
				return analyzeInternal(
					in puzzle,
					in solution,
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
					RuntimeAnalysisException e => e switch
					{
						WrongStepException
							=> result with { IsSolved = false, FailedReason = FailedReason.WrongStep, UnhandledException = e },
						PuzzleInvalidException
							=> result with { IsSolved = false, FailedReason = FailedReason.PuzzleIsInvalid }
					},
					OperationCanceledException { CancellationToken: var c } when c == cancellationToken
						=> result with { IsSolved = false, FailedReason = FailedReason.UserCancelled },
					NotImplementedException or NotSupportedException
						=> result with { IsSolved = false, FailedReason = FailedReason.NotImplemented },
					_
						=> result with { IsSolved = false, FailedReason = FailedReason.ExceptionThrown, UnhandledException = ex }
				};
			}
		}
		return result with { IsSolved = false, FailedReason = FailedReason.PuzzleHasNoSolution };


		AnalysisResult analyzeInternal(
			ref readonly Grid puzzle,
			ref readonly Grid solution,
			AnalysisResult resultBase,
			SymmetricType symmetricType,
			ReadOnlySpan<Digit?> mappingDigits,
			Mask selfPairedDigitsMask,
			IProgress<AnalyzerOrCollectorProgressPresenter>? progress,
			CancellationToken cancellationToken
		)
		{
			var playground = puzzle;
			var totalCandidatesCount = playground.CandidatesCount;
			var (collectedSteps, stepGrids, stepSearchers) = (new List<Step>(DefaultStepsCapacity), new List<Grid>(DefaultStepsCapacity), ResultStepSearchers);
			var timestampOriginal = Stopwatch.GetTimestamp();
			var accumulator = IsFullApplying
				|| RandomizedChoosing
#if SINGLE_TECHNIQUE_LIMIT_FLAG
				|| ConditionalOptions?.LimitedSingle is not (null or 0)
#endif
				? []
				: default(List<Step>);
			var context = new StepAnalysisContext(in playground, in puzzle)
			{
				Accumulator = accumulator,
				Options = Options,
				OnlyFindOne = !IsFullApplying
					&& !RandomizedChoosing
#if SINGLE_TECHNIQUE_LIMIT_FLAG
					&& ConditionalOptions?.LimitedSingle is null or 0
#endif
			};

			// Determine whether the grid is a GSP pattern. If so, check for eliminations.
			if ((symmetricType, selfPairedDigitsMask) is (not SymmetricType.None, not 0) && !mappingDigits.IsEmpty)
			{
				context.GspPatternInferred = symmetricType;
				context.MappingRelations = mappingDigits;

				if (SymmetryInferrer.GetStep(in playground, Options) is { } step)
				{
					if (verifyConclusionValidity(in solution, step))
					{
						if (onCollectingSteps(
							collectedSteps, step, in context, ref playground, timestampOriginal,
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
				switch (playground, solution, searcher, this)
				{
					case ({ PuzzleType: SudokuType.Sukaku }, _, { Metadata.SupportsSukaku: false }, _):
					case (_, _, { RunningArea: StepSearcherRunningArea.None }, _):
					case (_, _, { Metadata.IsConfiguredSlow: true }, { IgnoreSlowAlgorithms: true }):
					case (_, _, { Metadata.IsConfiguredHighAllocation: true }, { IgnoreHighAllocationAlgorithms: true }):
					case (_, _, { Metadata.IsOnlyRunForDirectViews: true }, { Options: { DistinctDirectMode: true, IsDirectMode: false } }):
					case (_, _, { Metadata.IsOnlyRunForIndirectViews: true }, { Options: { DistinctDirectMode: true, IsDirectMode: true } }):
					case (_, { IsUndefined: true }, { Metadata.SupportAnalyzingMultipleSolutionsPuzzle: false }, _):
					{
						// Skips on those two cases:
						// 1. Sukaku puzzles can't use techniques that is marked as "not supported for sukaku".
						// 2. If the searcher is currently disabled.
						// 3. If the searcher is configured as slow.
						// 4. If the searcher is configured as high-allocation.
						// 5. If the searcher is only run for direct view, and the current is indirect view.
						// 6. If the searcher is only run for indirect view, and the current is direct view.
						// 7. If the searcher doesn't support for analyzing puzzles with multiple solutions, but we enable it.
						continue;
					}
#if SINGLE_TECHNIQUE_LIMIT_FLAG
					case (_, _, SingleStepSearcher, { ConditionalOptions: { AllowsHiddenSingleInLines: var allowLine, LimitedSingle: var limited and not 0 } }):
					{
						accumulator!.Clear();

						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						// Special case: consider the step is a full house, hidden single or naked single,
						// ignoring steps not belonging to the technique set.
						var chosenSteps = new List<SingleStep>();
						foreach (var step in accumulator)
						{
							if (step is SingleStep { Code: var code } s)
							{
								switch (limited, code, allowLine)
								{
									case (SingleTechniqueFlag.FullHouse, not Technique.FullHouse, _):
									{
										break;
									}
									case (_, Technique.FullHouse, _):
									case (
										SingleTechniqueFlag.HiddenSingle,
										Technique.LastDigit or Technique.CrosshatchingBlock or Technique.HiddenSingleBlock,
										false
									):
									case (
										SingleTechniqueFlag.HiddenSingle,
										Technique.LastDigit
											or >= Technique.HiddenSingleBlock and <= Technique.HiddenSingleColumn
											or >= Technique.CrosshatchingBlock and <= Technique.CrosshatchingColumn,
										true
									):
									case (SingleTechniqueFlag.NakedSingle, Technique.NakedSingle, _):
									{
										chosenSteps.Add(s);
										break;
									}
								}
							}
						}
						if (chosenSteps.Count == 0)
						{
							continue;
						}

						if (IsFullApplying)
						{
							foreach (var step in chosenSteps)
							{
								if (!verifyConclusionValidity(in solution, step))
								{
									throw new WrongStepException(in playground, step);
								}

								if (onCollectingSteps(
									collectedSteps, step, in context, ref playground,
									timestampOriginal, stepGrids, resultBase, cancellationToken, out var result))
								{
									return result;
								}
							}
						}
						else
						{
							var chosenStep = RandomizedChoosing ? chosenSteps[_random.Next(0, chosenSteps.Count)] : chosenSteps[0];
							if (!verifyConclusionValidity(in solution, chosenStep))
							{
								throw new WrongStepException(in playground, chosenStep);
							}

							if (onCollectingSteps(
								collectedSteps, chosenStep, in context, ref playground,
								timestampOriginal, stepGrids, resultBase, cancellationToken, out var result))
							{
								return result;
							}
						}

						goto MakeProgress;
					}
#endif
					case (_, _, BruteForceStepSearcher, { RandomizedChoosing: true }):
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
							timestampOriginal, stepGrids, resultBase, cancellationToken, out var result))
						{
							return result;
						}

						goto MakeProgress;
					}
#if REMOVE_DUPLICATED_STEPS_IN_SINGLES_IF_RANDOM_ENABLED
					case (_, _, SingleStepSearcher, { RandomizedChoosing: true }):
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
							var distinctCandidatesKey = CandidateMap.Empty;
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
							timestampOriginal, stepGrids, resultBase, cancellationToken, out var result))
						{
							return result;
						}
						break;
					}
#endif
					case (_, _, not BruteForceStepSearcher, { IsFullApplying: true } or { RandomizedChoosing: true }):
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
								timestampOriginal, stepGrids, resultBase, cancellationToken, out var result))
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
									collectedSteps, foundStep, in context, ref playground, timestampOriginal, stepGrids,
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
										collectedSteps, foundStep, in context, ref playground, timestampOriginal, stepGrids,
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
				FailedReason = FailedReason.AnalyzerGiveUp,
				ElapsedTime = Stopwatch.GetElapsedTime(timestampOriginal),
				InterimSteps = [.. collectedSteps],
				InterimGrids = [.. stepGrids]
			};

		ReportStateAndTryNextStep:
			progress?.Report(new(progressedStepSearcherName, (double)(totalCandidatesCount - playground.CandidatesCount) / totalCandidatesCount));
			goto FindNextStep;


			static bool verifyConclusionValidity(ref readonly Grid solution, Step step)
			{
				if (solution.IsUndefined)
				{
					// This will be triggered when the puzzle has multiple solutions.
					return true;
				}

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
				ref readonly StepAnalysisContext context,
				ref Grid playground,
				long timestampOriginal,
				List<Grid> steppingGrids,
				AnalysisResult resultBase,
				CancellationToken cancellationToken,
				[NotNullWhen(true)] out AnalysisResult? result
			)
			{
				// Optimization: If the grid is inferred as a GSP pattern, we can directly add extra eliminations at symmetric positions.
				if (context is { GspPatternInferred: { } symmetricType } && step is not GurthSymmetricalPlacementStep)
				{
					var mappingRelations = context.MappingRelations;
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
					steppingGrids.AddRef(in playground);
					playground.Apply(step);
					steps.Add(step);

					if (playground.IsSolved)
					{
						result = resultBase with
						{
							IsSolved = true,
							Solution = playground,
							ElapsedTime = Stopwatch.GetElapsedTime(timestampOriginal),
							InterimSteps = [.. steps],
							InterimGrids = [.. steppingGrids]
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
