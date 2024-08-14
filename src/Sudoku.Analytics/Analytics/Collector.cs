namespace Sudoku.Analytics;

/// <summary>
/// Represents an instance that can collect all possible <see cref="Step"/>s in a grid for one state.
/// </summary>
public sealed partial class Collector : AnalyzerOrCollector, ICollector<Collector>
{
	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "MaxSteps", ParameterName = "count")]
	public int MaxStepsCollected { get; set; } = 1000;

	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "SameLevelConfiguration", ParameterName = "collectingMode")]
	public CollectorDifficultyLevelMode DifficultyLevelMode { get; set; } = CollectorDifficultyLevelMode.OnlySame;

	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "Culture", ParameterName = "culture")]
	public IFormatProvider? CurrentCulture { get; set; }

	/// <inheritdoc/>
	[FactoryProperty(ParameterType = typeof(StepSearcher[]), ParameterModifiers = "params")]
	[ImplicitField(RequiredReadOnlyModifier = false)]
	public override ReadOnlyMemory<StepSearcher> StepSearchers
	{
		get => _stepSearchers;

		set => ResultStepSearchers = FilterStepSearchers(_stepSearchers = value, StepSearcherRunningArea.Collecting);
	}

	/// <inheritdoc/>
	public override ReadOnlyMemory<StepSearcher> ResultStepSearchers { get; protected internal set; } =
		from searcher in StepSearcherPool.StepSearchers
		where searcher.RunningArea.HasFlag(StepSearcherRunningArea.Collecting)
		select searcher;

	/// <inheritdoc/>
	[FactoryProperty(MethodSuffixName = "UserDefinedOptions", ParameterName = "options")]
	public override StepSearcherOptions Options { get; set; } = StepSearcherOptions.Default;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when property <see cref="DifficultyLevelMode"/> is not defined.</exception>
	public ReadOnlySpan<Step> Collect(ref readonly CollectorContext context)
	{
		if (!Enum.IsDefined(DifficultyLevelMode))
		{
			throw new InvalidOperationException(SR.ExceptionMessage("ModeIsUndefined"));
		}

		ref readonly var puzzle = ref context.Puzzle;
		var progress = context.ProgressReporter;
		var cancellationToken = context.CancellationToken;

		if (puzzle.IsSolved)
		{
			return [];
		}

		try
		{
			return s(progress, in puzzle, cancellationToken);
		}
		catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
		{
			return null;
		}
		catch
		{
			throw;
		}


		ReadOnlySpan<Step> s(IProgress<AnalysisProgress>? progress, ref readonly Grid puzzle, CancellationToken ct)
		{
			const int defaultLevel = int.MaxValue;

			var possibleStepSearchers = ResultStepSearchers;
			var totalSearchersCount = possibleStepSearchers.Length;

			var playground = puzzle;
			Initialize(in playground, playground.GetSolutionGrid());

			var accumulator = new List<Step>();
			var context = new StepAnalysisContext(in playground, in puzzle) { Accumulator = accumulator, OnlyFindOne = false, Options = Options };
			var (l, bag, currentSearcherIndex) = (defaultLevel, new List<Step>(), 0);
			foreach (var searcher in possibleStepSearchers)
			{
				switch (searcher)
				{
					case { RunningArea: var runningArea } when !runningArea.HasFlag(StepSearcherRunningArea.Collecting):
					case { Metadata.SupportsSukaku: false } when puzzle.PuzzleType == SudokuType.Sukaku:
					{
						goto ReportProgress;
					}
					case { Level: var currentLevel }:
					{
						// If a searcher contains the upper level, it will be skipped.
						switch (DifficultyLevelMode)
						{
							case CollectorDifficultyLevelMode.OnlySame when l != defaultLevel && currentLevel <= l || l == defaultLevel:
							case CollectorDifficultyLevelMode.OneLevelHarder when l != defaultLevel && currentLevel <= l + 1 || l == defaultLevel:
							case CollectorDifficultyLevelMode.All:
							{
								break;
							}
							default:
							{
								goto ReportProgress;
							}
						}

						ct.ThrowIfCancellationRequested();

						// Searching.
						accumulator.Clear();

						searcher.Collect(ref context);

						if (accumulator.Count is not (var count and not 0))
						{
							goto ReportProgress;
						}

						l = currentLevel;
						bag.AddRange(count > MaxStepsCollected ? accumulator[..MaxStepsCollected] : accumulator);
						break;
					}
				}

			// Report the progress if worth.
			ReportProgress:
				progress?.Report(new(searcher.ToString(CurrentCulture), ++currentSearcherIndex / (double)totalSearchersCount));
			}

			// Return the result.
			return bag.AsReadOnlySpan();
		}
	}
}
