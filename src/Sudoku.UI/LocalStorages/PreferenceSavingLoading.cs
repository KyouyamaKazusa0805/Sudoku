namespace Sudoku.UI.LocalStorages;

/// <summary>
/// Provides a way to save or load preference files.
/// </summary>
internal static class PreferenceSavingLoading
{
	/// <summary>
	/// Indicates the config file name.
	/// </summary>
	private const string GlobalConfigFileName = $"Config{CommonFileExtensions.PreferenceBackup}";


	/// <summary>
	/// Indicates the configuration file local storage folder.
	/// </summary>
	private static string StorageFolder
		=> $"""{Environment.GetFolderPath(EnvironmentFolders.MyDocuments)}\{Program.ProgramName}""";


	/// <summary>
	/// Save the preference to the local path.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <returns>The task that holds the current operation.</returns>
	public static async Task SaveAsync(Preference up)
	{
		if (!SioDirectory.Exists(StorageFolder))
		{
			SioDirectory.CreateDirectory(StorageFolder);
		}

		string jsonBody = JsonSerializer.Serialize(up, CommonSerializerOptions.CamelCasing);
		await SioFile.WriteAllTextAsync(
			$"""{StorageFolder}\{GlobalConfigFileName}""",
			$"""
			{R["JsonConfigHeader"]}
			{jsonBody}
			"""
		);
	}

	/// <summary>
	/// Load the preference from the local path.
	/// </summary>
	/// <returns>
	/// A task that holds the current operation with a returning value of type <see cref="Preference"/>
	/// indicating the result value after the operation handled, or <see langword="null"/> if failed to fetch.
	/// </returns>
	public static async Task<Preference?> LoadAsync()
	{
		try
		{
			if (!SioDirectory.Exists(StorageFolder))
			{
				return null;
			}

			string path = $"""{StorageFolder}\{GlobalConfigFileName}""";
			if (!SioFile.Exists(path))
			{
				return null;
			}

			string content = await SioFile.ReadAllTextAsync(path);
			return string.IsNullOrWhiteSpace(content)
				? null
				: JsonSerializer.Deserialize<Preference>(content, CommonSerializerOptions.CamelCasing);
		}
		catch (Exception ex) when (ex is FileNotFoundException or JsonException or NotSupportedException)
		{
			return null;
		}
	}
}
