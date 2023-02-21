namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about drawing.
/// </summary>
internal static class DrawingConversion
{
	public static string GetModeString(DrawingMode drawingMode)
		=> GetString(
			drawingMode switch
			{
				DrawingMode.None => "_DrawingMode_None",
				DrawingMode.Cell => "_DrawingMode_Cell",
				DrawingMode.Candidate => "_DrawingMode_Candidate",
				DrawingMode.House => "_DrawingMode_House",
				DrawingMode.Chute => "_DrawingMode_Chute",
				DrawingMode.Link => "_DrawingMode_Link",
				DrawingMode.BabaGrouping => "_DrawingMode_BabaGrouping",
				_ => throw new ArgumentOutOfRangeException(nameof(drawingMode))
			}
		);

	public static Brush GetBrush(int currentColorIndex)
		=> ((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette switch
		{
			{ Count: var countOfColors } palette when currentColorIndex >= 0 && currentColorIndex < countOfColors
				=> new SolidColorBrush(palette[currentColorIndex]),
			_ => new SolidColorBrush()
		};
}
