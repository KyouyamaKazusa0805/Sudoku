using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.SourceGeneration;
using System.Timers;
using Sudoku.Algorithm.Symmetrical;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearchers;
using Sudoku.Concepts;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;

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
public sealed partial class Analyzer : AnalyzerOrCollector, IAnalyzer<Analyzer, AnalyzerResult>
{
	/// <summary>
	/// Indicates whether the solver will apply all found steps in a step searcher, in order to solve a puzzle faster.
	/// If the value is <see langword="true"/>, the third argument of <see cref="StepSearcher.Collect(ref AnalysisContext)"/>
	/// will be set <see langword="false"/> value, in order to find all possible steps in a step searcher,
	/// and all steps will be applied at the same time.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="StepSearcher.Collect(ref AnalysisContext)"/>
	public bool IsFullApplying { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="ConditionalFlags.TimeComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="ConditionalFlags.TimeComplexity"/>
	public bool IgnoreSlowAlgorithms { get; internal set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being configured <see cref="ConditionalFlags.SpaceComplexity"/>.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	/// <seealso cref="ConditionalFlags.SpaceComplexity"/>
	public bool IgnoreHighAllocationAlgorithms { get; internal set; }

	/// <inheritdoc/>
	[DisallowNull]
	[ImplicitField(RequiredReadOnlyModifier = false)]
	public override StepSearcher[]? StepSearchers
	{
		get => _stepSearchers;

		protected internal set => ResultStepSearchers = FilterStepSearchers(_stepSearchers = value, StepSearcherRunningArea.Searching);
	}

	/// <inheritdoc/>
	public override StepSearcher[] ResultStepSearchers { get; protected internal set; } =
		from searcher in StepSearcherPool.Default()
		where searcher.RunningArea.Flags(StepSearcherRunningArea.Searching)
		select searcher;

	/// <inheritdoc/>
	public override StepSearcherOptions Options { get; protected internal set; } = StepSearcherOptions.Default;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has already been solved.</exception>
	[UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "<Pending>")]
	public AnalyzerResult Analyze(scoped ref readonly Grid puzzle, IProgress<AnalyzerProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved)
		{
			throw new InvalidOperationException("This puzzle has already been solved.");
		}

		var result = new AnalyzerResult(in puzzle) { IsSolved = false };
		if (puzzle.ExactlyValidate(out var solution, out var sukaku) && sukaku is { } isSukaku)
		{
			// Firstly, we should check whether the puzzle is a GSP.
			puzzle.GetSymmetricalPlacementType(out var symmetricType, out var mappingDigits, out var selfPairedDigitsMask);

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
					NotImplementedException or NotSupportedException
						=> result with { IsSolved = false, FailedReason = FailedReason.NotImplemented },
					WrongStepException
						=> result with { IsSolved = false, FailedReason = FailedReason.WrongStep, UnhandledException = ex },
					OperationCanceledException { CancellationToken: var c } when c == cancellationToken
						=> result with { IsSolved = false, FailedReason = FailedReason.UserCancelled },
					_ when ex.GetType().IsGenericAssignableTo(typeof(StepSearcherProcessException<>))
						=> result with { IsSolved = false, FailedReason = FailedReason.PuzzleIsInvalid },
					_
						=> result with { IsSolved = false, FailedReason = FailedReason.ExceptionThrown, UnhandledException = ex }
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
			IProgress<AnalyzerProgress>? progress = null,
			CancellationToken cancellationToken = default
		)
		{
			var playground = puzzle;
			var totalCandidatesCount = playground.CandidatesCount;
			var (recordedSteps, stepGrids, stepSearchers) = (new List<Step>(100), new List<Grid>(100), ResultStepSearchers);
			string progressedStepSearcherName;
			scoped var stopwatch = ValueStopwatch.NewInstance;
			var accumulator = IsFullApplying ? (List<Step>)[] : null;
			scoped var context = new AnalysisContext(accumulator, ref playground, !IsFullApplying, Options);

			// Determine whether the grid is a GSP pattern. If so, check eliminations.
			if ((symmetricType, selfPairedDigitsMask) is (not SymmetricType.None, not 0) && !mappingDigits.IsEmpty)
			{
				(context.InferredGurthSymmetricalPlacementPattern, context.MappingRelations) = (symmetricType, [.. mappingDigits]);

				if (SymmetricalPlacementChecker.GetStep(in playground, Options) is { } step)
				{
					if (verifyConclusionValidity(in solution, step))
					{
						if (recordingStep(recordedSteps, step, in context, ref playground, in stopwatch, stepGrids, resultBase, cancellationToken, out var result))
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
						// 2. If the searcher is currently disabled.
						// 3. If the searcher is configured as slow.
						// 4. If the searcher is configured as high-allocation.
						continue;
					}
					case (_, not BruteForceStepSearcher, { IsFullApplying: true }):
					{
						accumulator!.Clear();

						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						foreach (var foundStep in accumulator)
						{
							if (verifyConclusionValidity(in solution, foundStep))
							{
								if (recordingStep(
									recordedSteps, foundStep, in context, ref playground, in stopwatch, stepGrids,
									resultBase, cancellationToken, out var result))
								{
									return result;
								}
							}
							else
							{
								throw new WrongStepException(in playground, foundStep);
							}
						}

						// The puzzle has not been finished, we should turn to the first step finder
						// to continue solving puzzle.
						goto AssignProgress;
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
									if (recordingStep(
										recordedSteps, foundStep, in context, ref playground, in stopwatch, stepGrids,
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
								goto AssignProgress;
							}
						}
					}
				}

			AssignProgress:
				progressedStepSearcherName = searcher.ToString();
				goto ReportStateAndTryToFindNextStep;
			}

			// All solver can't finish the puzzle... :(
			return resultBase with
			{
				FailedReason = FailedReason.PuzzleIsTooHard,
				ElapsedTime = stopwatch.ElapsedTime,
				Steps = [.. recordedSteps],
				SteppingGrids = [.. stepGrids]
			};

		ReportStateAndTryToFindNextStep:
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

			static bool recordingStep(
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
				if (context is { InferredGurthSymmetricalPlacementPattern: { } symmetricType, MappingRelations: { } mappingRelations }
					&& step is not GurthSymmetricalPlacementStep)
				{
					var copied = playground;
					var originalConclusions = step.Conclusions;
					step.Conclusions = [
						.. originalConclusions,
						..
						from conclusion in originalConclusions
						let newConclusion = conclusion.GetSymmetricConclusion(symmetricType, mappingRelations[conclusion.Digit] ?? -1)
						where newConclusion != conclusion
							&& copied.GetState(newConclusion.Cell) == CellState.Empty
							&& (copied.GetCandidates(newConclusion.Cell) >> newConclusion.Digit & 1) != 0
						select newConclusion
					];
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
