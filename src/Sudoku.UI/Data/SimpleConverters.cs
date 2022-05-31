namespace Sudoku.UI.Data;

/// <summary>
/// Provides with a set of methods as simple converters that can be used and called by XAML files.
/// </summary>
internal static class SimpleConverters
{
	public static string SliderPossibleValueString(double min, double max, double stepFrequency, double tickFrequency)
		=> $"{Get("SliderPossibleValue")}{min:0.0} - {max:0.0}{Get("SliderStepFrequency")}{stepFrequency:0.0}{Get("SliderTickFrequency")}{tickFrequency:0.0}";

	public static string SliderPossibleValueStringWithFormat(double min, double max, double stepFrequency, double tickFrequency, string format)
		=> $"{Get("SliderPossibleValue")}{min.ToString(format)} - {max.ToString(format)}{Get("SliderStepFrequency")}{stepFrequency.ToString(format)}{Get("SliderTickFrequency")}{tickFrequency.ToString(format)}";

	public static IList<string> GetFontNames()
		=> (from fontName in CanvasTextFormat.GetSystemFontFamilies() orderby fontName select fontName).ToList();

	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
}
