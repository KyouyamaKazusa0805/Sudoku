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
	public IEnumerable<IStep> Search(scoped in Grid puzzle, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved || !puzzle.ExactlyValidate(out _, out var sukaku))
		{
			return Array.Empty<IStep>();
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

						// TODO: Check whether the bag is too large (e.g. more than 1000 steps stored). We should skip the case in order to avoid some special and troublesome cases.
					}

					break;
				}
			}
		}

		// Return the result.
		return bag;
	}
}
