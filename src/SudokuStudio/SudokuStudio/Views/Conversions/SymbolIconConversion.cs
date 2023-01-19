namespace SudokuStudio.Views.Conversions;

internal static class SymbolIconConversion
{
	public static Symbol GetSymbolViaCandidateVisibility(bool displayCandidates) => displayCandidates ? Symbol.ZoomIn : Symbol.ZoomOut;
}
