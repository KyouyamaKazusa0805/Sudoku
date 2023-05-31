namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static Visibility CellStatusToValueTextBlockVisibility(CellStatus cellStatus)
		=> cellStatus is CellStatus.Modifiable or CellStatus.Given ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility CellStatusToCandidateTextBlockVisibility(CellStatus cellStatus, Mask candidatesMask, Digit digit)
		=> cellStatus == CellStatus.Empty && (candidatesMask >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;
}
