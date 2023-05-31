namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about color conversion.
/// </summary>
internal static class ColorConversion
{
	public static Brush GetBrush(Color color) => new SolidColorBrush(color);

	public static Brush? GetSudokuPaneBackground(bool transparent) => transparent ? null : new SolidColorBrush(Colors.White);
}
