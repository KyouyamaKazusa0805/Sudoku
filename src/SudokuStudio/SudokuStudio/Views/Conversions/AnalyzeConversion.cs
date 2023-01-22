namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about analyze tab pages.
/// </summary>
internal static class AnalyzeConversion
{
	public static bool GetIsEnabled(Grid grid) => !grid.GetSolution().IsUndefined;

	public static Visibility GetSummaryTableVisibility(IEnumerable itemsSource)
		=> itemsSource is null ? Visibility.Collapsed : Visibility.Visible;
}
