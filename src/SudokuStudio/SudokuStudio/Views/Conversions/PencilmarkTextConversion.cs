namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about pencilmark text.
/// </summary>
internal static class PencilmarkTextConversion
{
	public static double GetFontSize(double globalFontSize, double scale) => globalFontSize * scale;

	public static Brush GetBrush(Color pencilmarkColor) => new SolidColorBrush(pencilmarkColor);
}
