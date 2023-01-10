namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about value text.
/// </summary>
internal static class ValueTextConversion
{
	public static Brush GetValueFontColor(Grid grid, int cellIndex, Color modifiableColor, Color givenColor)
		=> new SolidColorBrush(grid.GetStatus(cellIndex) == CellStatus.Modifiable ? modifiableColor : givenColor);
}
