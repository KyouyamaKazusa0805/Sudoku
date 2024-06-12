namespace Sudoku.Analytics;

/// <summary>
/// Represents an instance that can collect all possible <see cref="Step"/>s in a grid for one state.
/// </summary>
public sealed partial class Collector : AnalyzerOrCollector
{
	/// <summary>
	/// Indicates the maximum steps can be gathered.
	/// </summary>
	/// <remarks>
	/// The default value is 1000.
	/// </remarks>
	public int MaxStepsGathered { get; set; } = 1000;

	/// <summary>
	/// Indicates whether the solver only displays the techniques with the same displaying level.
	/// </summary>
	/// <remarks>
	/// The default value is <see cref="CollectorDifficultyLevelMode.OnlySame"/>.
	/// </remarks>
	public CollectorDifficultyLevelMode DifficultyLevelMode { get; set; } = CollectorDifficultyLevelMode.OnlySame;

	/// <inheritdoc cref="Analyzer.CurrentCulture"/>
	public IFormatProvider? CurrentCulture { get; set; }

	/// <inheritdoc/>
	[ImplicitField(RequiredReadOnlyModifier = false)]
	public override ReadOnlyMemory<StepSearcher> StepSearchers
	{
		get => _stepSearchers;

		set => ResultStepSearchers = FilterStepSearchers(_stepSearchers = value, StepSearcherRunningArea.Collecting);
	}

	/// <inheritdoc/>
	public override ReadOnlyMemory<StepSearcher> ResultStepSearchers { get; protected internal set; } =
		from searcher in StepSearcherPool.BuiltInStepSearchers
		where searcher.RunningArea.HasFlag(StepSearcherRunningArea.Collecting)
		select searcher;

	/// <inheritdoc/>
	public override StepSearcherOptions Options { get; set; } = StepSearcherOptions.Default;


	/// <summary>
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="puzzle">The puzzle grid.</param>
	/// <param name="progress">The progress instance that is used for reporting the state.</param>
	/// <param name="cancellationToken">The cancellation token used for canceling an operation.</param>
	/// <returns>
	/// The result. If cancelled, the return value will be an empty instance; otherwise, a real list even though it may be empty.
	/// </returns>
	/// <exception cref="InvalidOperationException">Throws when property <see cref="DifficultyLevelMode"/> is not defined.</exception>
	public ReadOnlySpan<Step> Collect(ref readonly Grid puzzle, IProgress<AnalysisProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		if (!Enum.IsDefined(DifficultyLevelMode))
		{
			throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("ModeIsUndefined"));
		}

		if ((puzzle.PuzzleType == SudokuType.Sukaku) is var isSukaku && puzzle.IsSolved)
		{
			return [];
		}

		try
		{
			return s(isSukaku, progress, in puzzle, cancellationToken);
		}
		catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
		{
			return null;
		}
		catch
		{
			throw;
		}


		ReadOnlySpan<Step> s(bool isSukaku, IProgress<AnalysisProgress>? progress, ref readonly Grid puzzle, CancellationToken ct)
		{
			const int defaultLevel = int.MaxValue;

			var possibleStepSearchers = ResultStepSearchers;
			var totalSearchersCount = possibleStepSearchers.Length;

			var playground = puzzle;
			Initialize(in playground, playground.GetSolutionGrid());

			var accumulator = new List<Step>();
			var context = new AnalysisContext(in playground, in puzzle)
			{
				Accumulator = accumulator,
				OnlyFindOne = false,
				IsSukaku = isSukaku,
				Options = Options
			};
			var (l, bag, currentSearcherIndex) = (defaultLevel, new List<Step>(), 0);
			foreach (var searcher in possibleStepSearchers)
			{
				switch (searcher)
				{
					case { RunningArea: var runningArea } when !runningArea.HasFlag(StepSearcherRunningArea.Collecting):
					case { Metadata.SupportsSukaku: false } when isSukaku:
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
						bag.AddRange(count > MaxStepsGathered ? accumulator[..MaxStepsGathered] : accumulator);
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
