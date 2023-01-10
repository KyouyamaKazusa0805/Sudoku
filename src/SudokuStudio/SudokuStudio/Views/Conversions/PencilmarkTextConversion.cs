namespace SudokuStudio.Views.Conversions;

internal static class PencilmarkTextConversion
{
	public static double GetFontSize(double globalFontSize, double scale) => globalFontSize * scale;

	public static Brush GetColor(Color pencilmarkColor) => new SolidColorBrush(pencilmarkColor);
}
