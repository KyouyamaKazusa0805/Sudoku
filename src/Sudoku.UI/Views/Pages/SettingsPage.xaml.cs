namespace Sudoku.UI.Views.Pages;

/// <summary>
/// A page that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
[Page]
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage() => InitializeComponent();


	/// <summary>
	/// To open the folder which stores the preference file.
	/// </summary>
	private void OpenConfigFolder()
	{
		string folderName = $@"{Environment.GetFolderPath(EnvironmentFolders.MyDocuments)}\{Program.ProgramName}";
		if (!SioDirectory.Exists(folderName))
		{
			SimpleControlFactory.CreateErrorDialog(this, R["ErrorOpenFile"]!, R["ErrorOpenFile_Detail"]!);

			return;
		}

		try
		{
			Process.Start("explorer.exe", folderName);
		}
		catch (Exception ex) when (ex is InvalidOperationException or Win32Exception or FileNotFoundException)
		{
			SimpleControlFactory.CreateErrorDialog(this, R["ErrorOpenFile"]!, ex.Message);
		}
	}

	/// <summary>
	/// To backup a preference file.
	/// </summary>
	/// <returns>The task that handles the current operation.</returns>
	private async Task BackupPreferenceFileAsync()
	{
		var fsp = new FileSavePicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.WithSuggestedFileName(R["PreferenceBackup"]!)
			.AddFileTypeChoice(R["FileExtension_Configuration"]!, CommonFileExtensions.PreferenceBackup)
			.WithAwareHandleOnWin32();

		if (await fsp.PickSaveFileAsync() is not { Name: var fileName } file)
		{
			return;
		}

		// Prevent updates to the remote version of the file until we finish making changes
		// and call CompleteUpdatesAsync.
		CachedFileManager.DeferUpdates(file);

		// Writes to the file.
		var up = ((App)Application.Current).UserPreference;
		await FileIO.WriteTextAsync(file, Serialize(up, CamelCasing));

		// Let Windows know that we're finished changing the file so the other app can update
		// the remote version of the file.
		// Completing updates may require Windows to ask for user input.
		if (await CachedFileManager.CompleteUpdatesAsync(file) == FileUpdateStatus.Complete)
		{
			return;
		}

		// Failed to backup.
		string a = R["SettingsPage_BackupPreferenceFailed1"]!;
		string b = R["SettingsPage_BackupPreferenceFailed2"]!;
		await SimpleControlFactory.CreateErrorDialog(this, R["Info"]!, $"{a}{fileName}{b}").ShowAsync();
	}

	/// <summary>
	/// To load a preference file from local.
	/// </summary>
	/// <returns>The task that handles the current operation.</returns>
	private async Task LocalBackupPreferenceFromLocalAsync()
	{
		var fop = new FileOpenPicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.AddFileTypeFilter(CommonFileExtensions.PreferenceBackup)
			.WithAwareHandleOnWin32();

		var file = await fop.PickSingleFileAsync();
		if (file is not { Path: var filePath })
		{
			return;
		}

		if (new FileInfo(filePath).Length == 0)
		{
			SimpleControlFactory.CreateErrorDialog(this, string.Empty, R["SudokuPage_InfoBar_FileIsEmpty"]!);

			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await FileIO.ReadTextAsync(file);
		if (string.IsNullOrWhiteSpace(content))
		{
			SimpleControlFactory.CreateErrorDialog(this, string.Empty, R["SudokuPage_InfoBar_FileIsEmpty"]!);

			return;
		}

		try
		{
			var tempPref = Deserialize<Preference>(content, CamelCasing);

			((App)Application.Current).UserPreference.CoverPreferenceBy(tempPref);
		}
		catch (Exception ex) when (ex is JsonException or NotSupportedException)
		{
			SimpleControlFactory.CreateErrorDialog(this, string.Empty, R["SettingsPage_BackupPreferenceFailed_ParseFailed"]!);

			return;
		}
	}


	/// <summary>
	/// Triggers when the "open preference folder" button is clicked.
	/// </summary>
	/// <param name="sender">The object triggering the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void OpenPreferenceFolder_Click(object sender, RoutedEventArgs e) => OpenConfigFolder();

	/// <summary>
	/// Triggers when the "backup preference" button is clicked.
	/// </summary>
	/// <param name="sender">The object triggering the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void BackupPreference_ClickAsync(object sender, RoutedEventArgs e)
		=> await BackupPreferenceFileAsync();

	/// <summary>
	/// Triggers when the "load backup preference from local" button is clicked.
	/// </summary>
	/// <param name="sender">The object triggering the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void LoadBackupPreferenceFromLocal_ClickAsync(object sender, RoutedEventArgs e)
		=> await LocalBackupPreferenceFromLocalAsync();
}
