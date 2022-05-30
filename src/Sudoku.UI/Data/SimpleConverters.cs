namespace Sudoku.UI.Data;

internal static class SimpleConverters
{
	public static string SliderPossibleValueString(double min, double max, double stepFrequency, double tickFrequency)
		=> $"{Get("SliderPossibleValue")}{min:0.0} - {max:0.0}{Get("SliderStepFrequency")}{stepFrequency:0.0}{Get("SliderTickFrequency")}{tickFrequency:0.0}";

	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
}
