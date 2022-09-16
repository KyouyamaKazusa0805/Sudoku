namespace Sudoku.Solving.Logics.Implementations.StepGatherers;

/// <summary>
/// Defines a steps gatherer.
/// </summary>
public sealed class StepsGatherer : IStepGatherableSearcher, IStepGatherableSearcherOptions
{
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is 1000.
	/// </remarks>
	public int MaxStepsGathered { get; set; } = 1000;


	/// <inheritdoc/>
	public IEnumerable<IStep> Search(
		scoped in Grid puzzle,
		IProgress<double>? progress = null,
		CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved || !puzzle.ExactlyValidate(out _, out var sukaku))
		{
			return Array.Empty<IStep>();
		}

		const SearcherDisplayingLevel defaultLevelValue = (SearcherDisplayingLevel)255;

		var totalSearchersCount = StepSearcherPool.Collection.Length;

		InitializeMaps(puzzle);
		var i = defaultLevelValue;
		var bag = new List<IStep>();
		var currentSearcherIndex = 0;
		foreach (var searcher in StepSearcherPool.Collection)
		{
			switch (searcher)
			{
				case { Options.EnabledArea: var enabledArea } when !enabledArea.Flags(SearcherEnabledArea.Gathering):
				case { IsNotSupportedForSukaku: true } when sukaku.Value:
				{
					goto ReportProgress;
				}
				case { Options.DisplayingLevel: var currentLevel }:
				{
					// If a searcher contains the upper level, it will be skipped.
					if (OnlyShowSameLevelTechniquesInFindAllSteps && i != defaultLevelValue && i != currentLevel)
					{
						goto ReportProgress;
					}

					cancellationToken.ThrowIfCancellationRequested();

					// Searching.
					var tempBag = new List<IStep>();
					searcher.GetAll(tempBag, puzzle, false);

					switch (tempBag.Count)
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

							bag.AddRange(count > MaxStepsGathered ? tempBag.Slice(0, MaxStepsGathered) : tempBag);

							break;
						}
					}

					break;
				}
			}

			// Report the progress if worth.
		ReportProgress:
			progress?.Report(++currentSearcherIndex * 100D / totalSearchersCount);
		}

		// Return the result.
		return bag;
	}
}
