using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.SourceGeneration;
using System.Timers;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.StepSearchers;

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
	/// <exception cref="InvalidOperationException">Throws when the puzzle has already been solved.</exception>
	public AnalyzerResult Analyze(scoped in Grid puzzle, IProgress<AnalyzerProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved)
		{
			throw new InvalidOperationException("This puzzle has already been solved.");
		}

		var result = new AnalyzerResult(puzzle) { IsSolved = false };
		if (puzzle.ExactlyValidate(out var solution, out var sukaku) && sukaku is { } isSukaku)
		{
			try
			{
				return analyzeInternal(puzzle, solution, isSukaku, result, progress, cancellationToken);
			}
			catch (Exception ex)
			{
				return ex switch
				{
					NotImplementedException or NotSupportedException
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.NotImplemented },
					WrongStepException
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.WrongStep, UnhandledException = ex },
					OperationCanceledException { CancellationToken: var c } when c == cancellationToken
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.UserCancelled },
					_ when ex.GetType().IsGenericAssignableTo(typeof(StepSearcherProcessException<>))
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.PuzzleIsInvalid },
					_
						=> result with { IsSolved = false, FailedReason = AnalyzerFailedReason.ExceptionThrown, UnhandledException = ex }
				};
			}
		}
		return result with { IsSolved = false, FailedReason = AnalyzerFailedReason.PuzzleIsInvalid };


		AnalyzerResult analyzeInternal(
			scoped in Grid puzzle,
			scoped in Grid solution,
			bool isSukaku,
			AnalyzerResult resultBase,
			IProgress<AnalyzerProgress>? progress = null,
			CancellationToken cancellationToken = default
		)
		{
			var playground = puzzle;
			var totalCandidatesCount = playground.CandidatesCount;
			var (recordedSteps, stepGrids, stepSearchers) = (new List<Step>(100), new List<Grid>(100), ResultStepSearchers);
			string progressedStepSearcherName;
			scoped var stopwatch = ValueStopwatch.NewInstance;

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
						// 2. If the searcher is currently disabled.
						// 3. If the searcher is configured as slow.
						// 4. If the searcher is configured as high-allocation.
						continue;
					}
					case (_, not BruteForceStepSearcher, { IsFullApplying: true }):
					{
						var accumulator = new List<Step>();
						scoped var context = new AnalysisContext(accumulator, playground, false);
						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						foreach (var foundStep in accumulator)
						{
							if (verifyConclusionValidity(solution, foundStep))
							{
								if (recordingStep(
									recordedSteps, foundStep, ref playground, ref stopwatch, stepGrids,
									resultBase, cancellationToken, out var result))
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
						goto SetAnalyzerProgress;
					}
					default:
					{
						scoped var context = new AnalysisContext(null, playground, true);
						switch (searcher.Collect(ref context))
						{
							case null:
							{
								continue;
							}
							case var foundStep:
							{
								if (verifyConclusionValidity(solution, foundStep))
								{
									if (recordingStep(
										recordedSteps, foundStep, ref playground, ref stopwatch, stepGrids,
										resultBase, cancellationToken, out var result))
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
								goto SetAnalyzerProgress;
							}
						}
					}
				}

			SetAnalyzerProgress:
				progressedStepSearcherName = searcher.ToString();
				goto ReportStateAndSkipToTryAgain;
			}

			// All solver can't finish the puzzle... :(
			return resultBase with
			{
				FailedReason = AnalyzerFailedReason.PuzzleIsTooHard,
				ElapsedTime = stopwatch.ElapsedTime,
				Steps = [.. recordedSteps],
				StepGrids = [.. stepGrids]
			};

		ReportStateAndSkipToTryAgain:
			progress?.Report(new(progressedStepSearcherName, (double)(totalCandidatesCount - playground.CandidatesCount) / totalCandidatesCount));
			goto Again;


			static bool verifyConclusionValidity(scoped in Grid solution, Step step)
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
					stepGrids.Add(playground);
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
							StepGrids = [.. stepGrids]
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

	/// <summary>
	/// Only analyze for one step if <see cref="IsFullApplying"/> is <see langword="false"/>,
	/// or a list of <see cref="Step"/>s if <see cref="IsFullApplying"/> is <see langword="true"/>.
	/// </summary>
	/// <param name="puzzle">The puzzle.</param>
	/// <returns>The first steps found.</returns>
	/// <exception cref="InvalidOperationException">
	/// <inheritdoc cref="Analyze(in Grid, IProgress{AnalyzerProgress}?, CancellationToken)" path="/exception[@cref='InvalidOperationException']"/>
	/// </exception>
	/// <seealso cref="IsFullApplying"/>
	public Step[]? AnalyzeOneStep(scoped in Grid puzzle)
	{
		if (puzzle.IsSolved)
		{
			throw new InvalidOperationException("This puzzle has already been solved.");
		}

		if (puzzle.ExactlyValidate(out var solution, out var sukaku) && sukaku is { } isSukaku)
		{
			Initialize(puzzle, puzzle.SolutionGrid);
			foreach (var searcher in ResultStepSearchers)
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
						var accumulator = new List<Step>();
						scoped var context = new AnalysisContext(accumulator, puzzle, false);
						searcher.Collect(ref context);
						if (accumulator.Count == 0)
						{
							continue;
						}

						var steps = new List<Step>();
						foreach (var step in accumulator)
						{
							steps.Add(verifyConclusionValidity(solution, step) ? step : throw new WrongStepException(puzzle, step));
						}

						return [.. steps];
					}
					default:
					{
						scoped var context = new AnalysisContext(null, puzzle, true);
						if (searcher.Collect(ref context) is not { } step)
						{
							continue;
						}

						return verifyConclusionValidity(solution, step) ? [step] : throw new WrongStepException(puzzle, step);
					}
				}
			}
		}

		return null;


		static bool verifyConclusionValidity(scoped in Grid solution, Step step)
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
	}
}
