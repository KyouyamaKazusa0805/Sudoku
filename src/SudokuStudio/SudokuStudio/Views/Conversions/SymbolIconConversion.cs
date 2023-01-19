namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about symbol icon used by side bar buttons.
/// </summary>
internal static class SymbolIconConversion
{
	public static Symbol GetSymbolViaCandidateVisibility(bool displayCandidates) => displayCandidates ? Symbol.ZoomIn : Symbol.ZoomOut;
}
