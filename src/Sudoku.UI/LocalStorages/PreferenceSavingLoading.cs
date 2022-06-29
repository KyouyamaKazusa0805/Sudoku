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
	/// Indicates the configuration file local storage path.
	/// </summary>
	private static string StoragePath
#if WINDOWS_UI_STORAGE
	{
		get
		{
			var folder = ApplicationData.Current.LocalFolder;
			var file = await folder.CreateFileAsync(GlobalConfigFileName, CreationCollisionOption.ReplaceExisting);
			return file.Path;
		}
	}
#else
		=> SystemIOPath.Combine(Environment.GetFolderPath(EnvironmentFolders.MyDocuments), GlobalConfigFileName);
#endif


	/// <summary>
	/// Save the preference to the local path.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>The task that holds the current operation.</returns>
	public static async Task SaveAsync(Preference up, CancellationToken cancellationToken = default)
	{
		string json = JsonSerializer.Serialize(up, CommonReadOnlyFactory.DefaultSerializerOption);

#if WINDOWS_UI_STORAGE
		var folder = ApplicationData.Current.LocalFolder;
		var file = await folder.CreateFileAsync(GlobalConfigFileName, CreationCollisionOption.ReplaceExisting);
		await FileIO.WriteTextAsync(file, json);
#else
		await SystemIOFile.WriteAllTextAsync(StoragePath, json, cancellationToken);
#endif
	}

	/// <summary>
	/// Load the preference from the local path.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>
	/// A task that holds the current operation with a returning value of type <see cref="Preference"/>
	/// indicating the result value after the operation handled, or <see langword="null"/> if failed to fetch.
	/// </returns>
	public static async Task<Preference?> LoadAsync(CancellationToken cancellationToken = default)
	{
		try
		{
#if WINDOWS_UI_STORAGE
			var file = await ApplicationData.Current.LocalFolder.GetFileAsync(GlobalConfigFileName);
			string content = await FileIO.ReadTextAsync(file);
#else
			string content = await SystemIOFile.ReadAllTextAsync(StoragePath, cancellationToken);
#endif
			if (string.IsNullOrWhiteSpace(content))
			{
				goto ReturnNull;
			}

			var options = CommonReadOnlyFactory.DefaultSerializerOption;
			return JsonSerializer.Deserialize<Preference>(content, options);
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex) when (ex is JsonException or NotSupportedException)
		{
		}

	ReturnNull:
		return null;
	}
}
