namespace SudokuStudio.Views.Conversions;

internal static class AnalyzeConversion
{
	public static bool GetIsEnabled(Grid grid) => !grid.GetSolution().IsUndefined;
}
