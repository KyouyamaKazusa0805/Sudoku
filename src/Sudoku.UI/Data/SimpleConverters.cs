namespace Sudoku.UI.Data;

internal static class SimpleConverters
{
	public static string SliderPossibleValueString(double min, double max)
		=> $"{Get("SliderPossibleValue")}{min:0.0} - {max:0.0}";

	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;

	public static Thickness SettingItemToMargin(string? s)
		=> StringToVisibility(s) == Visibility.Collapsed ? new(20, 56, 0, 0) : new(20, 84, 0, 0);
}
