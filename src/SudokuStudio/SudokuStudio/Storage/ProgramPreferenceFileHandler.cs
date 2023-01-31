namespace SudokuStudio.Storage;

/// <summary>
/// Defines a handler that handles the file of file extension <see cref="CommonFileExtensions.UserPreference"/>.
/// </summary>
/// <seealso cref="CommonFileExtensions.UserPreference"/>
public sealed class ProgramPreferenceFileHandler : IProgramSupportedFileHandler<ProgramPreference>
{
	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, DiagnosticId = "SCA0108", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0108")]
	private ProgramPreferenceFileHandler() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string SupportedFileExtension => CommonFileExtensions.UserPreference;


	/// <inheritdoc/>
	public static ProgramPreference? Read(string filePath)
		=> Deserialize<ProgramPreference>(File.ReadAllText(filePath), CommonSerializerOptions.CamelCasingSupportsField);

	/// <inheritdoc/>
	public static void Write(string filePath, ProgramPreference instance)
	{
		var directory = SystemPath.GetDirectoryName(filePath)!;
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(filePath, Serialize(instance, CommonSerializerOptions.CamelCasingSupportsField));
	}
}
