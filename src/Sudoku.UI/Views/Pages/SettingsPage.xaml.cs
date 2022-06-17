namespace Sudoku.UI.Views.Pages;

/// <summary>
/// A page that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
[Page]
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// Indicates the backing list of <see cref="SettingGroupItem"/>s.
	/// </summary>
	private readonly IList<SettingGroupItem> _settingGroupItems = new SettingGroupItem[]
	{
		new()
		{
			Name = R["SettingsPage_GroupItemName_Basic"]!,
			Description = R["SettingsPage_GroupItemDescription_Basic"]!,
			SettingItem = new SettingItem[]
			{
				new ToggleSwitchSettingItem
				{
					Name = R["SettingsPage_ItemName_ShowCandidates"]!,
					PreferenceValueName = nameof(Preference.ShowCandidates)
				},
				new ToggleSwitchSettingItem
				{
					Name = R["SettingsPage_ItemName_ShowCandidateBorderLines"]!,
					PreferenceValueName = nameof(Preference.ShowCandidateBorderLines)
				},
				new ToggleSwitchSettingItem
				{
					Name = R["SettingsPage_ItemName_AllowFocusing"]!,
					Description = R["SettingsPage_ItemDescription_AllowFocusing"]!,
					PreferenceValueName = nameof(Preference.AllowFocusing)
				},
				new ToggleSwitchSettingItem
				{
					Name = R["SettingsPage_ItemName_EnableDeltaValuesDisplaying"]!,
					Description = R["SettingsPage_ItemDescription_EnableDeltaValuesDisplaying"]!,
					PreferenceValueName = nameof(Preference.EnableDeltaValuesDisplaying)
				},
				new ToggleSwitchSettingItem
				{
					Name = R["SettingsPage_ItemName_PlaceholderIsZero"]!,
					Description = R["SettingsPage_ItemDescription_PlaceholderIsZero"]!,
					PreferenceValueName = nameof(Preference.PlaceholderIsZero),
					OnContent = R["SettingsPage_ItemName_PlaceholderIsZero_OnContent"]!,
					OffContent = R["SettingsPage_ItemName_PlaceholderIsZero_OffContent"]!
				},
				new FontPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_ValueFontScale"]!,
					Description = R["SettingsPage_ItemDescription_ValueFontScale"]!,
					PreferenceValueName = nameof(Preference.ValueFontName),
					FontScalePropertyName = nameof(Preference.ValueFontScale)
				},
				new FontPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_CandidateFontScale"]!,
					Description = R["SettingsPage_ItemDescription_CandidateFontScale"]!,
					PreferenceValueName = nameof(Preference.CandidateFontName),
					FontScalePropertyName = nameof(Preference.CandidateFontScale)
				}
			}
		},
		new()
		{
			Name = R["SettingsPage_GroupItemName_Solving"]!,
			Description = R["SettingsPage_GroupItemDescription_Solving"]!
		},
		new()
		{
			Name = R["SettingsPage_GroupItemName_Rendering"]!,
			Description = R["SettingsPage_GroupItemDescription_Rendering"]!,
			SettingItem = new SettingItem[]
			{
				new SliderSettingItem
				{
					Name = R["SettingsPage_ItemName_OutsideBorderWidth"]!,
					Description = R["SettingsPage_ItemDescription_OutsideBorderWidth"]!,
					PreferenceValueName = nameof(Preference.OutsideBorderWidth),
					StepFrequency = .1,
					TickFrequency = .3,
					MinValue = 0,
					MaxValue = 3
				},
				new SliderSettingItem
				{
					Name = R["SettingsPage_ItemName_BlockBorderWidth"]!,
					PreferenceValueName = nameof(Preference.BlockBorderWidth),
					StepFrequency = .5,
					TickFrequency = .5,
					MinValue = 0,
					MaxValue = 5
				},
				new SliderSettingItem
				{
					Name = R["SettingsPage_ItemName_CellBorderWidth"]!,
					PreferenceValueName = nameof(Preference.CellBorderWidth),
					StepFrequency = .5,
					TickFrequency = .5,
					MinValue = 0,
					MaxValue = 5
				},
				new SliderSettingItem
				{
					Name = R["SettingsPage_ItemName_CandidateBorderWidth"]!,
					Description = R["SettingsPage_ItemDescription_CandidateBorderWidth"]!,
					PreferenceValueName = nameof(Preference.CandidateBorderWidth),
					StepFrequency = .1,
					TickFrequency = .3,
					MinValue = 0,
					MaxValue = 3
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_OutsideBorderColor"]!,
					PreferenceValueName = nameof(Preference.OutsideBorderColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_BlockBorderColor"]!,
					PreferenceValueName = nameof(Preference.BlockBorderColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_CellBorderColor"]!,
					PreferenceValueName = nameof(Preference.CellBorderColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_CandidateBorderColor"]!,
					Description = R["SettingsPage_ItemDescription_CandidateBorderColor"]!,
					PreferenceValueName = nameof(Preference.CandidateBorderColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_GivenColor"]!,
					PreferenceValueName = nameof(Preference.GivenColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_ModifiableColor"]!,
					PreferenceValueName = nameof(Preference.ModifiableColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_CandidateColor"]!,
					PreferenceValueName = nameof(Preference.CandidateColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_CellDeltaColor"]!,
					Description = R["SettingsPage_ItemDescription_CellDeltaColor"]!,
					PreferenceValueName = nameof(Preference.CellDeltaColor)
				},
				new ColorPickerSettingItem
				{
					Name = R["SettingsPage_ItemName_CandidateDeltaColor"]!,
					Description = R["SettingsPage_ItemDescription_CandidateDeltaColor"]!,
					PreferenceValueName = nameof(Preference.CandidateDeltaColor)
				}
			}
		},
		new()
		{
			Name = R["SettingsPage_GroupItemName_Miscellaneous"]!,
			Description = R["SettingsPage_GroupItemDescription_Miscellaneous"]!,
			SettingItem = new[]
			{
				new ToggleSwitchSettingItem
				{
					Name = R["SettingsPage_ItemName_DescendingOrderedInfoBarBoard"]!,
					PreferenceValueName = nameof(Preference.DescendingOrderedInfoBarBoard)
				}
			}
		}
	};


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage() => InitializeComponent();

	/// <summary>
	/// To backup a preference file.
	/// </summary>
	/// <returns>The task that handles the current operation.</returns>
	private async Task BackupPreferenceFileAsync()
	{
		var fsp = new FileSavePicker
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
			SuggestedFileName = R["PreferenceBackup"]!
		};
		fsp.FileTypeChoices.Add(R["FileExtension_Configuration"], new List<string> { CommonFileExtensions.PreferenceBackup });
		fsp.AwareHandleOnWin32();

		if (await fsp.PickSaveFileAsync() is not { Name: var fileName } file)
		{
			return;
		}

		// Prevent updates to the remote version of the file until we finish making changes
		// and call CompleteUpdatesAsync.
		CachedFileManager.DeferUpdates(file);

		// Writes to the file.
		await FileIO.WriteTextAsync(
			file,
			JsonSerializer.Serialize(
				((App)Application.Current).UserPreference,
				CommonReadOnlyFactory.DefaultSerializerOption
			)
		);

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
	/// Triggers when the "backup preference" button is clicked.
	/// </summary>
	/// <param name="sender">The object triggering the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void BackupPreference_ClickAsync(object sender, RoutedEventArgs e) => await BackupPreferenceFileAsync();
}
