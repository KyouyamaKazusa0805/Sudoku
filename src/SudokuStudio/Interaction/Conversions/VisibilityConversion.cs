namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static double CellStateToValueTextBlockOpacity(CellState cellState)
		=> cellState is CellState.Modifiable or CellState.Given ? 1 : 0;

	public static double CellStateToCandidateTextBlockOpacity(Grid grid, Cell cell, Digit digit, ViewUnitBindableSource? viewUnit, bool displayCandidates)
	{
		if (grid.Exists(cell, digit) is not true)
		{
			// This cell doesn't contain such candidate, or this cell is not empty.
			return 0;
		}

		// If the cell contains such candidate, we should check whether the candidate should be shown.
		// If the current mode shows for all candidates, or a view unit shows for such candidate, we should show it;
		// otherwise, hide.
		if (viewUnit?.CandidateContains(cell * 9 + digit) ?? false)
		{
			return 1;
		}

		// Fallback to check option on displaying candidates.
		return displayCandidates ? 1 : 0;
	}
}
