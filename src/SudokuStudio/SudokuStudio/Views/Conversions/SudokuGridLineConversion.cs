namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about grid lines in <see cref="SudokuPane"/> control.
/// </summary>
public static class SudokuGridLineConversion
{
	public static Brush GetBrush(Color borderColor) => new SolidColorBrush(borderColor);
}
