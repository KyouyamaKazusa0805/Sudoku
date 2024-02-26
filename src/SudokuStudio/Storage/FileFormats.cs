namespace SudokuStudio.Storage;

/// <summary>
/// Represents file formats.
/// </summary>
public static class FileFormats
{
	/// <summary>
	/// Indicates the plain text format.
	/// </summary>
	public static readonly FileFormat PlainText = new(ResourceDictionary.Get("FileExtension_PlainTextDescription", App.CurrentCulture), FileExtensions.PlainText);

	/// <summary>
	/// Indicates the sudoku text file format (SST - Sudoku Studio Text format).
	/// </summary>
	public static readonly FileFormat Text = new(ResourceDictionary.Get("FileExtension_TextDescription", App.CurrentCulture), FileExtensions.Text);

	/// <summary>
	/// Indicates the comma-separated values file format.
	/// </summary>
	public static readonly FileFormat CommaSeparated = new(ResourceDictionary.Get("FileExtension_CommaSeparated", App.CurrentCulture), FileExtensions.CommaSeparated);

	/// <summary>
	/// Indicates the portable picture format.
	/// </summary>
	public static readonly FileFormat PortablePicture = new(ResourceDictionary.Get("FileExtension_Picture", App.CurrentCulture), FileExtensions.PortablePicture);

	/// <summary>
	/// Indicates the portable document format.
	/// </summary>
	public static readonly FileFormat PortableDocument = new(ResourceDictionary.Get("FileExtension_PortableDocument", App.CurrentCulture), FileExtensions.PortableDocument);

	/// <summary>
	/// Indicates the sudoku generating history file format (SSG - Sudoku Studio Generating format).
	/// </summary>
	public static readonly FileFormat GeneratingHistory = new(ResourceDictionary.Get("FileExtension_GeneratingHistory", App.CurrentCulture), FileExtensions.GeneratingHistory);

	/// <summary>
	/// Indicates the sudoku grid library file format (SSL - Sudoku Studio Library format).
	/// </summary>
	public static readonly FileFormat PuzzleLibrary = new(ResourceDictionary.Get("FileExtension_GridLibrary", App.CurrentCulture), FileExtensions.PuzzleLibrary);

	/// <summary>
	/// Indicates the user preference file format (SSP - Sudoku Studio Preference format).
	/// </summary>
	public static readonly FileFormat UserPreference = new(ResourceDictionary.Get("FileExtension_UserPreference", App.CurrentCulture), FileExtensions.UserPreference);


	/// <summary>
	/// Add a new format into the collection of file-type choices for a <see cref="FileSavePicker"/> instance.
	/// </summary>
	/// <param name="this">The file save picker.</param>
	/// <param name="format">The format to be added.</param>
	public static void AddFileFormat(this FileSavePicker @this, FileFormat format)
		=> @this.FileTypeChoices.Add(format.Description, format.Formats);

	/// <summary>
	/// Add a new format into the filtering collection for a <see cref="FileOpenPicker"/> instance.
	/// </summary>
	/// <param name="this">The file open picker.</param>
	/// <param name="format">The format to be added.</param>
	/// <exception cref="InvalidOperationException">Throws when the format value is invalid.</exception>
	public static void AddFileFormat(this FileOpenPicker @this, FileFormat format)
		=> @this.FileTypeFilter.Add(
			format.Formats is [var f]
				? f
				: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("FileFormatContainsMultipleSubformats"))
		);
}
