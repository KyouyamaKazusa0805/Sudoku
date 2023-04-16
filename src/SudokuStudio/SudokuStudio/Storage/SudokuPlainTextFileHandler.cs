namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="CommonFileExtensions.PlainText"/>.
/// </summary>
/// <seealso cref="CommonFileExtensions.PlainText"/>
public sealed class SudokuPlainTextFileHandler : IProgramSupportedFileHandler<Grid>
{
	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false)]
	private SudokuPlainTextFileHandler() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string SupportedFileExtension => CommonFileExtensions.PlainText;


	/// <inheritdoc/>
	public static Grid Read(string filePath) => Grid.Parse(File.ReadAllText(filePath));

	/// <inheritdoc/>
	public static void Write(string filePath, Grid instance) => File.WriteAllText(filePath, instance.ToString("#"));
}
