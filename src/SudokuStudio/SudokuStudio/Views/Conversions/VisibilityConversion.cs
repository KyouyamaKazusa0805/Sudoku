namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static Visibility CellStatusToValueTextBlockVisibility(CellStatus cellStatus)
		=> cellStatus is CellStatus.Modifiable or CellStatus.Given ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility CellStatusToCandidateTextBlockVisibility(CellStatus cellStatus, short candidatesMask, int digit)
		=> cellStatus == CellStatus.Empty && (candidatesMask >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility CoordinateLinesVisibility(bool showCoordinateLines)
		=> showCoordinateLines ? Visibility.Visible : Visibility.Collapsed;
}
