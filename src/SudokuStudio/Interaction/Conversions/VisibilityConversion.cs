namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about visibility.
/// </summary>
internal static class VisibilityConversion
{
	public static double CellStatusToValueTextBlockOpacity(CellStatus cellStatus)
		=> cellStatus is CellStatus.Modifiable or CellStatus.Given ? 1 : 0;

	public static double CellStatusToCandidateTextBlockOpacity(CellStatus cellStatus, Mask candidatesMask, Digit digit)
		=> cellStatus == CellStatus.Empty && (candidatesMask >> digit & 1) != 0 ? 1 : 0;
}
