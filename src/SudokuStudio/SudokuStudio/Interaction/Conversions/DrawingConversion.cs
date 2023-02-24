namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about drawing.
/// </summary>
internal static class DrawingConversion
{
	public static int GetDrawingModeIndex(DrawingMode drawingMode) => (int)(drawingMode - 1);

	public static int GetLinkTypeIndex(Inference inference)
		=> inference switch
		{
			Inference.Strong => 0,
			Inference.Weak => 1,
			Inference.Default => 2
		};

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);

	public static Brush GetSelectedBrush(int currentColorIndex)
		=> ((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette switch
		{
			{ Count: var countOfColors } palette when currentColorIndex >= 0 && currentColorIndex < countOfColors
				=> new SolidColorBrush(palette[currentColorIndex]),
			_ => new SolidColorBrush()
		};
}
