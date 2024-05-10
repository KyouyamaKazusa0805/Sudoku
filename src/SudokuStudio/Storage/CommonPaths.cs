namespace SudokuStudio.Storage;

/// <summary>
/// Provides with some paths that is used for the whole program.
/// </summary>
internal static class CommonPaths
{
	/// <summary>
	/// Indicates the sudoku studio name.
	/// </summary>
	private const string SudokuStudioName = nameof(SudokuStudio);


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

	/// <summary>
	/// Indicates cache folder path.
	/// </summary>
	public static readonly string Cache;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static CommonPaths()
	{
		var documents = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		UserPreference = $@"{documents}\{SudokuStudioName}\user-preference{FileExtensions.UserPreference}";
		GeneratingHistory = $@"{documents}\{SudokuStudioName}\generating-history{FileExtensions.GeneratingHistory}";
		Library = $@"{documents}\{SudokuStudioName}\libraries";
		Cache = $@"{documents}\{SudokuStudioName}\cache";
	}


	/// <summary>
	/// Creates the target path if the path doesn't exist.
	/// </summary>
	/// <param name="path">The path.</param>
	public static void CreateIfNotExist(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}
}
