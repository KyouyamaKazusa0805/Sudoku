namespace SudokuStudio.Views.Conversions;

internal static class ValueTextConversion
{
	public static Brush GetValueFontColor(Grid grid, int cellIndex, Color modifiableColor, Color givenColor)
		=> new SolidColorBrush(grid.GetStatus(cellIndex) == CellStatus.Modifiable ? modifiableColor : givenColor);
}
