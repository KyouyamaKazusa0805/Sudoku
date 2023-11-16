using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Fish</b> step searcher.
/// </summary>
/// <param name="priority">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='priority']"/>
/// </param>
/// <param name="level">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='level']"/>
/// </param>
/// <param name="runningArea">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='runningArea']"/>
/// </param>
public abstract class FishStepSearcher(
	int priority,
	int level,
	StepSearcherRunningArea runningArea = StepSearcherRunningArea.Searching | StepSearcherRunningArea.Collecting
) : StepSearcher(priority, level, runningArea)
{
	/// <summary>
	/// Check whether the fish is sashimi.
	/// </summary>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="fins">All fins.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// <para>A <see cref="bool"/> value indicating that.</para>
	/// <para>
	/// <inheritdoc cref="ComplexFishStep.IsSashimi" path="/remarks"/>
	/// </para>
	/// </returns>
	protected static bool? IsSashimi(House[] baseSets, scoped ref readonly CellMap fins, Digit digit)
	{
		if (!fins)
		{
			return null;
		}

		var isSashimi = false;
		foreach (var baseSet in baseSets)
		{
			if ((HousesMap[baseSet] - fins & CandidatesMap[digit]).Count == 1)
			{
				isSashimi = true;
				break;
			}
		}

		return isSashimi;
	}
}
