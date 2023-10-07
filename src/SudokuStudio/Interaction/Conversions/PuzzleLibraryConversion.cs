using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Interaction.Conversions;

internal static class PuzzleLibraryConversion
{
	public static string GetLoadingOrAddingDialogTitle(bool isAdding)
		=> GetString(isAdding ? "LibraryPage_AddLibraryTitle" : "LibraryPage_LoadLibraryTitle");

	public static string GetTags(string[] tags) => string.Format(GetString("LibraryPage_TagsAre"), string.Join(GetString("_Token_Comma"), tags));

	public static string GetPuzzlesCountText(int count)
		=> string.Format(GetString(count == 1 ? "LibraryPage_PuzzlesCountIsSingular" : "LibraryPage_PuzzlesCountIsPlural"), count);
}
