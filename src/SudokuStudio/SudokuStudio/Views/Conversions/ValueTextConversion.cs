namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about value text.
/// </summary>
internal static class ValueTextConversion
{
	public static string GetText(CellStatus cellStatus, short candidatesMask)
		=> cellStatus == CellStatus.Empty || candidatesMask is not (>= 0 and < 511)
			? string.Empty
			: (TrailingZeroCount((uint)candidatesMask) + 1).ToString();

	public static Brush GetValueFontColor(Grid grid, int cellIndex, Color modifiableColor, Color givenColor)
		=> new SolidColorBrush(grid.GetStatus(cellIndex) == CellStatus.Modifiable ? modifiableColor : givenColor);
}
