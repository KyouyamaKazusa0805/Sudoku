using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a fish module.
/// </summary>
internal static class FishModule
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

	/// <summary>
	/// Try to get the number of body cells.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The number of cells of body, in fact.</returns>
	public static int GetBodyCellsCount(scoped ref readonly Grid grid, House[] baseSets, Digit digit)
	{
		var bodyCellsCount = 0;
		foreach (var baseSet in baseSets)
		{
			bodyCellsCount += (CandidatesMap[digit] & HousesMap[baseSet]).Count;
		}

		return bodyCellsCount;
	}
}
