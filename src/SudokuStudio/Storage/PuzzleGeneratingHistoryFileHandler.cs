namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="FileExtensions.GeneratingHistory"/>.
/// </summary>
/// <seealso cref="FileExtensions.GeneratingHistory"/>
public sealed class PuzzleGeneratingHistoryFileHandler : IProgramSupportedFileHandler<PuzzleGenertingHistory>
{
	/// <inheritdoc/>
	public static string SupportedFileExtension => FileExtensions.GeneratingHistory;


	/// <inheritdoc/>
	public static PuzzleGenertingHistory? Read(string filePath)
		=> JsonSerializer.Deserialize<PuzzleGenertingHistory>(File.ReadAllText(filePath), CommonSerializerOptions.PascalCasing);

	/// <inheritdoc/>
	public static void Write(string filePath, PuzzleGenertingHistory instance)
		=> File.WriteAllText(filePath, JsonSerializer.Serialize(instance, CommonSerializerOptions.PascalCasing));
}
