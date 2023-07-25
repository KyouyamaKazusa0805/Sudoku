namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static double CellStatusToValueTextBlockOpacity(CellStatus cellStatus)
		=> cellStatus is CellStatus.Modifiable or CellStatus.Given ? 1 : 0;

	public static double CellStatusToCandidateTextBlockOpacity(CellStatus cellStatus, bool displayCandidates)
		=> cellStatus == CellStatus.Empty && displayCandidates ? 1 : 0;
}
