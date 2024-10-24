namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about generator pages.
/// </summary>
internal static class GeneratingConversion
{
	public static bool GetProgressRingIsActive_GeneratingPage(bool isGeneratorLaunched) => isGeneratorLaunched;

	public static bool GetProgressRingIsIntermediate_GeneratingPage(bool isGeneratorLaunched) => isGeneratorLaunched;

	public static bool GetIsEnabledForGoToAnalyzePage(Grid puzzle) => !puzzle.IsEmpty && puzzle.ModifiableCellsCount == 0;

	public static Visibility GetProgressRingVisibility_GeneratingPage(bool isGeneratorLaunched)
		=> isGeneratorLaunched ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility GetAnalyzeTabsVisibility_GeneratingPage(bool isGeneratorLaunched)
		=> isGeneratorLaunched ? Visibility.Collapsed : Visibility.Visible;

	public static Brush GetPatternCounterBrush(CellMap selectedCells)
		=> new SolidColorBrush(selectedCells.Count < 17 ? Colors.Red : App.CurrentTheme == ApplicationTheme.Light ? Colors.Black : Colors.White);
}
