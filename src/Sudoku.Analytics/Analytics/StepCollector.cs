namespace Sudoku.Analytics;

/// <summary>
/// Represents an instance that can collect all possible <see cref="Step"/>s in a grid for one status.
/// </summary>
public sealed class StepCollector : IAnalyzerOrCollector
{
	/// <summary>
	/// The backing field of property <see cref="StepSearchers"/>.
	/// </summary>
	/// <seealso cref="StepSearchers"/>
	private StepSearcher[]? _stepSearchers;


	/// <summary>
	/// Indicates whether the solver only displays the techniques with the same displaying level.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool OnlyShowSameLevelTechniquesInFindAllSteps { get; internal set; } = true;

	/// <summary>
	/// Indicates the maximum steps can be gathered.
	/// </summary>
	/// <remarks>
	/// The default value is 1000.
	/// </remarks>
	public int MaxStepsGathered { get; internal set; } = 1000;

	/// <inheritdoc/>
	[DisallowNull]
	public StepSearcher[]? StepSearchers
	{
		get => _stepSearchers;

		internal set => ResultStepSearchers = IAnalyzerOrCollector.FilterStepSearchers(_stepSearchers = value, StepSearcherRunningArea.Gathering);
	}

	/// <inheritdoc/>
	public StepSearcher[] ResultStepSearchers { get; private set; } =
	(
		from searcher in StepSearcherPool.Default(true)
		where searcher.RunningArea.Flags(StepSearcherRunningArea.Gathering)
		select searcher
	).ToArray();


	/// <summary>
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="puzzle">The puzzle grid.</param>
	/// <param name="progress">The progress instance that is used for reporting the status.</param>
	/// <param name="cancellationToken">The cancellation token used for canceling an operation.</param>
	/// <returns>The result.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	public IEnumerable<Step> Search(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved || !puzzle.ExactlyValidate(out _, out var sukaku))
		{
			return Array.Empty<Step>();
		}

		const int defaultLevelValue = int.MaxValue;

		var possibleStepSearchers = ResultStepSearchers;
		var totalSearchersCount = possibleStepSearchers.Length;

		Initialize(puzzle, puzzle.SolutionGrid);

		var (i, bag, currentSearcherIndex) = (defaultLevelValue, new List<Step>(), 0);
		foreach (var searcher in possibleStepSearchers)
		{
			switch (searcher)
			{
				case { RunningArea: var runningArea } when !runningArea.Flags(StepSearcherRunningArea.Gathering):
				case { IsNotSupportedForSukaku: true } when sukaku.Value:
				{
					goto ReportProgress;
				}
				case { Level: var currentLevel }:
				{
					// If a searcher contains the upper level, it will be skipped.
					if (OnlyShowSameLevelTechniquesInFindAllSteps && i != defaultLevelValue && i != currentLevel)
					{
						goto ReportProgress;
					}

					cancellationToken.ThrowIfCancellationRequested();

					// Searching.
					var accumulator = new List<Step>();
					scoped var context = new AnalysisContext(accumulator, puzzle, false);
					searcher.Collect(ref context);

					switch (accumulator.Count)
					{
						case 0:
						{
							goto ReportProgress;
						}
						case var count:
						{
							if (OnlyShowSameLevelTechniquesInFindAllSteps)
							{
								i = currentLevel;
							}

							bag.AddRange(count > MaxStepsGathered ? accumulator.Slice(0, MaxStepsGathered) : accumulator);

							break;
						}
					}

					break;
				}
			}

		// Report the progress if worth.
		ReportProgress:
			progress?.Report(++currentSearcherIndex / (double)totalSearchersCount);
		}

		// Return the result.
		return bag;
	}
}
