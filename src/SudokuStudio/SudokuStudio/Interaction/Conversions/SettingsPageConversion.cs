namespace SudokuStudio.Interaction.Conversions;

internal static class SettingsPageConversion
{
	public static string GetSliderString(double value, string format) => value.ToString(format);

	public static string ToRgbString(Color color)
	{
		var (r, g, b) = color;
		return $"{r}, {g}, {b}";
	}

	public static CoordinateLabelDisplayMode GetCoordinateLabelDisplayMode(int index) => (CoordinateLabelDisplayMode)index;

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);

	public static FontFamily GetFont(string fontName) => new(fontName);
}
