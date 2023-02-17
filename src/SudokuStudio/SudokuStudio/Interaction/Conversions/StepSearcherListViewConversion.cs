namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="StepSearcherListView"/>.
/// </summary>
/// <seealso cref="StepSearcherListView"/>
internal static class StepSearcherListViewConversion
{
	public static Brush GetIsEnabledBrush(SearcherEnabledArea enabledArea, SearcherEnabledArea currentArea, bool isTemporarilyDisabled)
		=> new SolidColorBrush(enabledArea.Flags(currentArea) && !isTemporarilyDisabled ? Colors.Black : Colors.Gray);
}
