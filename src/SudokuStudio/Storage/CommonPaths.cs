namespace SudokuStudio.Storage;

/// <summary>
/// Provides with some paths that is used for the whole program.
/// </summary>
internal static class CommonPaths
{
	/// <summary>
	/// Indicates the user preference path.
	/// </summary>
	public static readonly string UserPreference;

	/// <summary>
	/// Indicates puzzle-generating history path.
	/// </summary>
	public static readonly string GeneratingHistory;

	/// <summary>
	/// Indicates puzzle-library folder path.
	/// </summary>
	public static readonly string Library;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static CommonPaths()
	{
		var documents = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		UserPreference = $@"{documents}\{nameof(SudokuStudio)}\user-preference{FileExtensions.UserPreference}";
		GeneratingHistory = $@"{documents}\{nameof(SudokuStudio)}\generating-history{FileExtensions.GeneratingHistory}";
		Library = $@"{documents}\{nameof(SudokuStudio)}\libraries";
	}
}
