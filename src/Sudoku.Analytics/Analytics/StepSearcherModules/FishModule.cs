using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearchers;
using Sudoku.Concepts;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a fish module.
/// </summary>
internal sealed class FishModule : IStepSearcherModule<FishModule>
{
	/// <inheritdoc/>
	static Type[] IStepSearcherModule<FishModule>.SupportedTypes => [typeof(NormalFishStepSearcher), typeof(ComplexFishStepSearcher)];


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
	public static bool? IsSashimi(House[] baseSets, scoped ref readonly CellMap fins, Digit digit)
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
