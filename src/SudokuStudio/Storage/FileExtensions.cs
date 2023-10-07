namespace SudokuStudio.Storage;

/// <summary>
/// Provides a list of file extensions of bound file types used by this project.
/// This type provides with values that can to be used by <see cref="FileFormat"/> instances.
/// </summary>
/// <seealso cref="FileFormat"/>
public static class FileExtensions
{
	/// <summary>
	/// Indicates the plain text format.
	/// </summary>
	public const string PlainText = ".txt";

	/// <summary>
	/// Indicates the portable picture format.
	/// </summary>
	public const string PortablePicture = ".png";

	/// <summary>
	/// Indicates the sudoku generating history file format (SSG - Sudoku Studio Generating format).
	/// </summary>
	public const string GeneratingHistory = ".ssg";

	/// <summary>
	/// Indicates the sudoku grid library file format (SSL - Sudoku Studio Library format).
	/// </summary>
	public const string PuzzleLibrary = ".ssl";

	/// <summary>
	/// Indicates the sudoku text file format (SST - Sudoku Studio Text format).
	/// </summary>
	public const string Text = ".sst";

	/// <summary>
	/// Indicates the comma-separated text file format.
	/// </summary>
	public const string CommaSeparated = ".csv";

	/// <summary>
	/// Indicates the user preference file format (SSP - Sudoku Studio Preference format).
	/// </summary>
	public const string UserPreference = ".ssp";

	/// <summary>
	/// Indicates the portable document format.
	/// </summary>
	public const string PortableDocument = ".pdf";
}
