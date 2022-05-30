namespace Sudoku.UI.Data;

internal static class SimpleConverters
{
	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;

	public static Thickness SettingItemToMargin(string? s)
		=> StringToVisibility(s) == Visibility.Collapsed ? new(20, 32, 0, 0) : new(20, 60, 0, 0);
}
