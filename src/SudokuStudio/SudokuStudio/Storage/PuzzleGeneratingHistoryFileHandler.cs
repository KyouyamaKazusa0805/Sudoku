namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="CommonFileExtensions.GeneratingHistory"/>.
/// </summary>
/// <seealso cref="CommonFileExtensions.GeneratingHistory"/>
public sealed class PuzzleGeneratingHistoryFileHandler : IProgramSupportedFileHandler<PuzzleGenertingHistory>
{
	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, DiagnosticId = "SCA0108", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0108")]
	private PuzzleGeneratingHistoryFileHandler() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string SupportedFileExtension => CommonFileExtensions.GeneratingHistory;


	/// <inheritdoc/>
	public static PuzzleGenertingHistory? Read(string filePath)
		=> Deserialize<PuzzleGenertingHistory>(File.ReadAllText(filePath), CommonSerializerOptions.CamelCasing);

	/// <inheritdoc/>
	public static void Write(string filePath, PuzzleGenertingHistory instance)
		=> File.WriteAllText(filePath, Serialize(instance, CommonSerializerOptions.CamelCasing));
}
