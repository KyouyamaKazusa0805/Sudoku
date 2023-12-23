namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="SingleCountingPracticingPage"/>.
/// </summary>
/// <seealso cref="SingleCountingPracticingPage"/>
internal static class SingleCountingPracticingPageConversion
{
	public static bool GetIsEnabled(bool isRunning) => !isRunning;

	public static Visibility GetResultDisplayerVisibility(bool isRunning) => isRunning ? Visibility.Collapsed : Visibility.Visible;
}
