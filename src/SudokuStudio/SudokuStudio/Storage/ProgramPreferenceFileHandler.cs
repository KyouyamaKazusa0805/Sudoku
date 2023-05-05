namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="CommonFileExtensions.UserPreference"/>.
/// </summary>
/// <seealso cref="CommonFileExtensions.UserPreference"/>
public sealed class ProgramPreferenceFileHandler : IProgramSupportedFileHandler<ProgramPreference>
{
	/// <summary>
	/// Indicates the default options to be used by <see cref="JsonSerializer"/>.
	/// </summary>
	private static readonly JsonSerializerOptions Options = new(CommonSerializerOptions.PascalCasing) { IncludeFields = true };


	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false)]
	private ProgramPreferenceFileHandler() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string SupportedFileExtension => CommonFileExtensions.UserPreference;


	/// <inheritdoc/>
	public static ProgramPreference? Read(string filePath) => Deserialize<ProgramPreference>(File.ReadAllText(filePath), Options);

	/// <inheritdoc/>
	public static void Write(string filePath, ProgramPreference instance)
	{
		var directory = SystemPath.GetDirectoryName(filePath)!;
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(filePath, Serialize(instance, Options));
	}
}
