namespace SudokuStudio.Interaction.Conversions;

internal static class SettingsPageConversion
{
	public static string GetSliderString(double value, string format) => value.ToString(format);
}
