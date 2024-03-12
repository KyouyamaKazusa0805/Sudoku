namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="FileExtensions.UserPreference"/>.
/// </summary>
/// <seealso cref="FileExtensions.UserPreference"/>
public sealed class ProgramPreferenceFileHandler : IProgramSupportedFileHandler<ProgramPreference>
{
	/// <summary>
	/// Indicates the default options to be used by <see cref="JsonSerializer"/>.
	/// </summary>
	private static readonly JsonSerializerOptions Options = new(CommonSerializerOptions.PascalCasing)
	{
		IncludeFields = true,
		IgnoreReadOnlyProperties = false,
		Converters = { new JsonStringEnumConverter(PascalCaseJsonNamingPolicy.PascalCase, true), new RangeConverter() },
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
	};


	/// <inheritdoc/>
	public static string SupportedFileExtension => FileExtensions.UserPreference;


	/// <inheritdoc/>
	public static ProgramPreference? Read(string filePath) => JsonSerializer.Deserialize<ProgramPreference>(File.ReadAllText(filePath), Options);

	/// <inheritdoc/>
	public static void Write(string filePath, ProgramPreference instance)
	{
		var directory = io::Path.GetDirectoryName(filePath)!;
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(filePath, JsonSerializer.Serialize(instance, Options));
	}
}
