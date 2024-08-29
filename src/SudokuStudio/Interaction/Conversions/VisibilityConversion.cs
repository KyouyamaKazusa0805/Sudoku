namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static double CellStateToValueTextBlockOpacity(CellState cellState)
		=> cellState is CellState.Modifiable or CellState.Given ? 1 : 0;

	public static double CellStateToCandidateTextBlockOpacity(Cell cell, Digit digit, CellState cellState, ViewUnitBindableSource? viewUnit, bool displayCandidates)
		=> cellState == CellState.Empty && displayCandidates || (viewUnit?.CandidateContains(cell * 9 + digit) ?? false) ? 1 : 0;
}
