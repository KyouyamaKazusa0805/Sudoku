namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="FileExtensions.PlainText"/>.
/// </summary>
/// <seealso cref="FileExtensions.PlainText"/>
public sealed class SudokuPlainTextFileHandler : IProgramSupportedFileHandler<Grid>
{
	/// <inheritdoc/>
	public static string SupportedFileExtension => FileExtensions.PlainText;


	/// <inheritdoc/>
	public static Grid Read(string filePath) => Grid.Parse(File.ReadAllText(filePath));

	/// <inheritdoc/>
	public static void Write(string filePath, Grid instance)
	{
		var character = ((App)Application.Current).Preference.UIPreferences.EmptyCellCharacter;
		File.WriteAllText(filePath, instance.ToString($"#{character}"));
	}
}
