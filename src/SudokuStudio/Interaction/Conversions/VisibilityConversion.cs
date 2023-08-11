namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static double CellStatusToValueTextBlockOpacity(CellState cellStatus)
		=> cellStatus is CellState.Modifiable or CellState.Given ? 1 : 0;

	public static double CellStatusToCandidateTextBlockOpacity(Cell cell, Digit digit, CellState cellStatus, CandidateMap usedCandidatesInViewUnit, bool displayCandidates)
		=> cellStatus == CellState.Empty && displayCandidates || usedCandidatesInViewUnit.Contains(cell * 9 + digit) ? 1 : 0;
}
