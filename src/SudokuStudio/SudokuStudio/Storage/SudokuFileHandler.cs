namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="CommonFileExtensions.Text"/>.
/// </summary>
/// <seealso cref="CommonFileExtensions.Text"/>
public sealed class SudokuFileHandler : IProgramSupportedFileHandler<GridInfo[]>
{
	/// <summary>
	/// Indicates the default options to be used by <see cref="JsonSerializer"/>.
	/// </summary>
	private static readonly JsonSerializerOptions Options = new(CommonSerializerOptions.CamelCasing) { IgnoreReadOnlyProperties = false };


	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false)]
	private SudokuFileHandler() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string SupportedFileExtension => CommonFileExtensions.Text;


	/// <inheritdoc/>
	public static GridInfo[]? Read(string filePath) => Deserialize<GridInfo[]>(File.ReadAllText(filePath), Options);

	/// <inheritdoc/>
	public static void Write(string filePath, GridInfo[] instance) => File.WriteAllText(filePath, Serialize(instance, Options));
}
