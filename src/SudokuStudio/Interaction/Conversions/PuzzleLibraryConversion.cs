using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Interaction.Conversions;

internal static class PuzzleLibraryConversion
{
	public static int GetModeRawValue(LibraryDataUpdatingMode mode) => (int)mode;

	public static string GetLoadingOrAddingDialogTitle(LibraryDataUpdatingMode mode)
		=> GetString(
			mode switch
			{
				LibraryDataUpdatingMode.Add => "LibraryPage_AddLibraryTitle",
				LibraryDataUpdatingMode.Load => "LibraryPage_LoadLibraryTitle",
				LibraryDataUpdatingMode.Update => "LibraryPage_UpdateLibraryTitle"
			}
		);

	public static string GetTags(string[] tags) => string.Format(GetString("LibraryPage_TagsAre"), string.Join(GetString("_Token_Comma"), tags));

	public static string GetPuzzlesCountText(int count)
		=> string.Format(GetString(count == 1 ? "LibraryPage_PuzzlesCountIsSingular" : "LibraryPage_PuzzlesCountIsPlural"), count);
}
