namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines a steps gatherer.
/// </summary>
public sealed partial class StepsGatherer
{
	/// <summary>
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="puzzle">The puzzle grid.</param>
	/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
	/// <returns>The result grouped by technique names.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
	public IEnumerable<IGrouping<string, Step>> Search(in Grid puzzle, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved || !puzzle.IsValid(out _, out bool? sukaku))
		{
			return Array.Empty<IGrouping<string, Step>>();
		}

		InitializeMaps(puzzle);
		var i = (DisplayingLevel)255;
		var bag = new List<Step>();
		foreach (var searcher in StepSearcherPool.Collection)
		{
			switch (searcher)
			{
				case { Options.EnabledAreas: var enabledAreas } when !enabledAreas.Flags(EnabledAreas.Gathering):
				{
					// Skip the searcher that is disabled in this case.
					continue;
				}
				case { Identifier: SearcherIdentifier.DeadlyPattern } when sukaku.Value:
				{
					// UR searchers will be disabled in sukaku mode.
					continue;
				}
				case { Options.DisplayingLevel: var currentLevel }:
				{
					// Check the level of the searcher.
					// If a searcher contains the upper level value than the current searcher holding,
					// the searcher will be skipped to search steps.
					if (OnlyShowSameLevelTechniquesInFindAllSteps)
					{
						if (i != (DisplayingLevel)255 && i != currentLevel)
						{
							continue;
						}
					}

					cancellationToken.ThrowIfCancellationRequested();

					// Searching.
					var tempBag = new List<Step>();
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
