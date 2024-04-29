namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about generator pages.
/// </summary>
internal static class GeneratingConversion
{
	public static bool GetProgressRingIsActive_GeneratingPage(bool isGeneratorLaunched) => isGeneratorLaunched;

	public static bool GetProgressRingIsIntermediate_GeneratingPage(bool isGeneratorLaunched) => isGeneratorLaunched;

	public static Visibility GetProgressRingVisibility_GeneratingPage(bool isGeneratorLaunched)
		=> isGeneratorLaunched ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility GetAnalyzeTabsVisibility_GeneratingPage(bool isGeneratorLaunched)
		=> isGeneratorLaunched ? Visibility.Collapsed : Visibility.Visible;
}
