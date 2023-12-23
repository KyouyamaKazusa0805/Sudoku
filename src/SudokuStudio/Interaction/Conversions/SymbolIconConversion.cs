namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about symbol icon used by side bar buttons.
/// </summary>
internal static class SymbolIconConversion
{
	public static IconElement GetSymbolViaCandidateVisibility(bool displayCandidates)
		=> new SymbolIcon(displayCandidates ? Symbol.ZoomIn : Symbol.ZoomOut);
}
