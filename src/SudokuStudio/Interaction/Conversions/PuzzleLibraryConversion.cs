namespace SudokuStudio.Interaction.Conversions;

internal static class PuzzleLibraryConversion
{
	/// <summary>
	/// The internal analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = PredefinedAnalyzers.SstsOnly;


	public static int GetModeRawValue(LibraryDataUpdatingMode mode) => (int)mode;

	public static int GetCurrentPuzzleIndexInIntegerBox(int currentPuzzleIndex) => currentPuzzleIndex + 1;

	public static string GetLoadingOrAddingDialogTitle(LibraryDataUpdatingMode mode)
		=> ResourceDictionary.Get(
			mode switch
			{
				LibraryDataUpdatingMode.Add => "LibraryPage_AddLibraryTitle",
				LibraryDataUpdatingMode.Load => "LibraryPage_LoadLibraryTitle",
				LibraryDataUpdatingMode.Update => "LibraryPage_UpdateLibraryTitle",
				LibraryDataUpdatingMode.AddOne => "LibraryPage_AddOneLibraryTitle"
			},
			App.CurrentCulture
		);

	public static string GetTags(string[] tags)
		=> string.Format(
			ResourceDictionary.Get("LibraryPage_TagsAre", App.CurrentCulture),
			string.Join(ResourceDictionary.Get("_Token_Comma", App.CurrentCulture), tags)
		);

	public static string GetPuzzlesCountText(int count)
		=> string.Format(ResourceDictionary.Get(count == 1 ? "LibraryPage_PuzzlesCountIsSingular" : "LibraryPage_PuzzlesCountIsPlural", App.CurrentCulture), count);

	public static string GetLibraryName(string libName, string libFileId) => $"{libName} ({libFileId}{FileExtensions.PuzzleLibrary})";
}
