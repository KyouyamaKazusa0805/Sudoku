namespace Sudoku.UI.Data;

internal static class SimpleConverters
{
	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
}
