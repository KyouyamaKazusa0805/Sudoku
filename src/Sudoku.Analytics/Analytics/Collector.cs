namespace Sudoku.Analytics;

/// <summary>
/// Represents an instance that can collect all possible <see cref="Step"/>s in a grid for one state.
/// </summary>
public sealed class Collector :
	ICollector<Collector, CollectorContext, ReadOnlySpan<Step>>,
	meta::ICollector<Grid, Step>
{
	/// <inheritdoc/>
	[WithProperty(MethodSuffixName = "MaxSteps", ParameterName = "count")]
	public int MaxStepsCollected { get; set; } = 1000;

	/// <summary>
	/// Indicates the difficulty level mode that the step searcher will be called and checked.
	/// </summary>
	[WithProperty(MethodSuffixName = "SameLevelConfiguration", ParameterName = "collectingMode")]
	public CollectorDifficultyLevelMode DifficultyLevelMode { get; set; } = CollectorDifficultyLevelMode.OnlySame;

	/// <inheritdoc/>
	[WithProperty(ParameterType = typeof(StepSearcher[]), ParameterModifiers = "params")]
	public ReadOnlyMemory<StepSearcher> StepSearchers
	{
		get => field;

		set => ResultStepSearchers = ICollector<Collector, CollectorContext, ReadOnlySpan<Step>>.FilterStepSearchers(field = value, StepSearcherRunningArea.Collecting);
	}

	/// <inheritdoc/>
	public ReadOnlyMemory<StepSearcher> ResultStepSearchers { get; internal set; } =
		from searcher in StepSearcherFactory.StepSearchers
		where searcher.RunningArea.HasFlag(StepSearcherRunningArea.Collecting)
		select searcher;

	/// <inheritdoc/>
	[WithProperty(MethodSuffixName = "UserDefinedOptions", ParameterName = "options")]
	public StepGathererOptions Options { get; set; } = StepGathererOptions.Default;

	/// <inheritdoc/>
	[AddProperty(AllowsMultipleAdding = true, MethodSuffixName = "StepSearcherSetter")]
	public ICollection<Action<StepSearcher>> Setters { get; } = [];


	/// <inheritdoc cref="Collect(ref readonly CollectorContext)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Step> Collect(ref readonly Grid grid) => Collect(new CollectorContext(in grid));

	/// <inheritdoc/>
	public ReadOnlySpan<Step> Collect(scoped ref readonly CollectorContext context)
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

		ICollector<Collector, CollectorContext, ReadOnlySpan<Step>>.ApplySetters(this);

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


		ReadOnlySpan<Step> s(IProgress<StepGathererProgressPresenter>? progress, ref readonly Grid puzzle, CancellationToken ct)
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
				progress?.Report(new(searcher.ToString(Options.CurrentCulture), ++currentSearcherIndex / (double)totalSearchersCount));
			}

			// Return the result.
			return bag.AsSpan();
		}
	}

#pragma warning disable CS9087
	/// <inheritdoc/>
	unsafe ReadOnlySpan<Step> meta::ICollector<Grid, Step>.Collect(Grid board, CancellationToken cancellationToken)
		=> Collect(new CollectorContext(in board) { CancellationToken = cancellationToken });
#pragma warning restore CS9087
}
