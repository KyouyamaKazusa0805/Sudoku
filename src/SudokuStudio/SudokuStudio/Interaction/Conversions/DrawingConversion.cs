namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about drawing.
/// </summary>
internal static class DrawingConversion
{
	public static int GetDrawingModeIndex(DrawingMode drawingMode) => (int)(drawingMode - 1);

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);

	public static Brush GetSelectedBrush(int currentColorIndex)
		=> ((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette switch
		{
			{ Count: var countOfColors } palette when currentColorIndex >= 0 && currentColorIndex < countOfColors
				=> new SolidColorBrush(palette[currentColorIndex]),
			_ => new SolidColorBrush()
		};
}
