namespace SudokuStudio.Interaction.Conversions;

internal static class LibraryConversion
{
	public static string GetDisplayName(string? libName, string libFileId)
		=> libName is not null
			? $"{libName} ({libFileId}{FileExtensions.PuzzleLibrary})"
			: $"{libFileId}{FileExtensions.PuzzleLibrary}";
}
