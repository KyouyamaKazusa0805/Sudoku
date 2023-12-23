namespace SudokuStudio.Interaction.Conversions;

internal static class PuzzleLibraryConversion
{
	/// <summary>
	/// The internal analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = PredefinedAnalyzers.SstsOnly;


	public static bool GetCandidatesVisibility(PuzzleLibraryBindableSource? source, int currentPuzzleIndex)
	{
		if (source is null)
		{
			return false;
		}

		var currentPuzzle = source.Puzzles[currentPuzzleIndex];
		return ((App)Application.Current).Preference.LibraryPreferences.LibraryCandidatesVisibility switch
		{
			LibraryCandidatesVisibility.ShownWhenPuzzleIsGreaterThanModerate => !Analyzer.Analyze(in currentPuzzle).IsSolved,
			LibraryCandidatesVisibility.ShownWhenExtraEliminatedCandidatesFound => currentPuzzle.ToString("#").Contains(':'),
			LibraryCandidatesVisibility.AlwaysShown => true,
			_ => false
		};
	}

	public static int GetModeRawValue(LibraryDataUpdatingMode mode) => (int)mode;

	public static int GetTotalPagesCount(PuzzleLibraryBindableSource? source) => source?.Puzzles.Length ?? -1;

	public static int GetCurrentPuzzleIndexInIntegerBox(int currentPuzzleIndex) => currentPuzzleIndex + 1;

	public static string GetTotalPagesCountText(PuzzleLibraryBindableSource? source) => $"/ {GetTotalPagesCount(source)}";

	public static string GetLoadingOrAddingDialogTitle(LibraryDataUpdatingMode mode)
		=> GetString(
			mode switch
			{
				LibraryDataUpdatingMode.Add => "LibraryPage_AddLibraryTitle",
				LibraryDataUpdatingMode.Load => "LibraryPage_LoadLibraryTitle",
				LibraryDataUpdatingMode.Update => "LibraryPage_UpdateLibraryTitle",
				LibraryDataUpdatingMode.AddOne => "LibraryPage_AddOneLibraryTitle"
			}
		);

	public static string GetTags(string[] tags)
		=> string.Format(GetString("LibraryPage_TagsAre"), string.Join(GetString("_Token_Comma"), tags));

	public static string GetPuzzlesCountText(int count)
		=> string.Format(GetString(count == 1 ? "LibraryPage_PuzzlesCountIsSingular" : "LibraryPage_PuzzlesCountIsPlural"), count);

	public static string GetLibraryName(string libName, string libFileId) => $"{libName} ({libFileId}{FileExtensions.PuzzleLibrary})";

	public static Visibility GetPreviousButtonVisibility(PuzzleLibraryBindableSource? source, int currentPuzzleIndex)
		=> source is null ? Visibility.Collapsed : currentPuzzleIndex == 0 ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetNextButtonVisibility(PuzzleLibraryBindableSource? source, int currentPuzzleIndex)
		=> source is null ? Visibility.Collapsed : currentPuzzleIndex == source.PuzzlesCount - 1 ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetPagingControlsVisibility(PuzzleLibraryBindableSource? source)
		=> source?.Puzzles.Length switch { null or 0 or 1 => Visibility.Collapsed, _ => Visibility.Visible };

	public static Grid GetCurrentGrid(PuzzleLibraryBindableSource source, int puzzleIndex) => source.Puzzles[puzzleIndex];
}
