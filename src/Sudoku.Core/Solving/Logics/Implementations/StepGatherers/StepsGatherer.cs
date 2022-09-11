namespace Sudoku.Solving.Implementations.StepGatherers;

/// <summary>
/// Defines a steps gatherer.
/// </summary>
public sealed class StepsGatherer :
	IStepGatherableSearcher<IGrouping<string, IStep>, string, IStep>,
	IStepsGathererOptions
{
	/// <inheritdoc/>
	public bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; }


	/// <inheritdoc/>
	public IEnumerable<IGrouping<string, IStep>> Search(scoped in Grid puzzle, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved || !puzzle.ExactlyValidate(out _, out var sukaku))
		{
			return Array.Empty<IGrouping<string, IStep>>();
		}

		InitializeMaps(puzzle);
		var i = (SearcherDisplayingLevel)255;
		var bag = new List<IStep>();
		foreach (var searcher in StepSearcherPool.Collection)
		{
			switch (searcher)
			{
				case { Options.EnabledArea: var enabledArea } when !enabledArea.Flags(SearcherEnabledArea.Gathering):
				case { IsNotSupportedForSukaku: true } when sukaku.Value:
				{
					continue;
				}
				case { Options.DisplayingLevel: var currentLevel }:
				{
					// Check the level of the searcher.
					// If a searcher contains the upper level value than the current searcher holding,
					// the searcher will be skipped to search steps.
					if (OnlyShowSameLevelTechniquesInFindAllSteps)
					{
						if (i != (SearcherDisplayingLevel)255 && i != currentLevel)
						{
							continue;
						}
					}

					cancellationToken.ThrowIfCancellationRequested();

					// Searching.
					var tempBag = new List<IStep>();
					searcher.GetAll(tempBag, puzzle, false);

					// Gather the technique steps, and record the current level of the searcher.
					if (tempBag.Count != 0)
					{
						if (OnlyShowSameLevelTechniquesInFindAllSteps)
						{
							i = currentLevel;
						}

						bag.AddRange(tempBag);
					}

					break;
				}
			}
		}

		// Return the result.
		return from step in bag group step by step.Name;
	}
}
