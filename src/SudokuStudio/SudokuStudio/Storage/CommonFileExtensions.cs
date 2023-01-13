namespace SudokuStudio.Storage;

/// <summary>
/// Provides a list of file extension names of bound file types used by this project.
/// </summary>
internal static class CommonFileExtensions
{
	/// <summary>
	/// Indicates the plain text format.
	/// </summary>
	public const string PlainText = ".txt";

	/// <summary>
	/// Indicates the sudoku text file format (SST - Sudoku Studio Text format).
	/// </summary>
	public const string Text = ".sst";

	/// <summary>
	/// Indicates the user preference file format (SSP - Sudoku Studio Preference format).
	/// </summary>
	public const string UserPreference = ".ssp";
}
