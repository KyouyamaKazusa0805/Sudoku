namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="CommonFileExtensions.Text"/>.
/// </summary>
/// <seealso cref="CommonFileExtensions.Text"/>
public sealed class SudokuFileHandler : IProgramSupportedFileHandler<GridSerializationData[]>
{
	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false)]
	private SudokuFileHandler() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string SupportedFileExtension => CommonFileExtensions.Text;


	/// <inheritdoc/>
	public static GridSerializationData[]? Read(string filePath)
		=> Deserialize<GridSerializationData[]>(File.ReadAllText(filePath), CommonSerializerOptions.CamelCasing);

	/// <inheritdoc/>
	public static void Write(string filePath, GridSerializationData[] instance)
		=> File.WriteAllText(filePath, Serialize(instance, CommonSerializerOptions.CamelCasing));
}
