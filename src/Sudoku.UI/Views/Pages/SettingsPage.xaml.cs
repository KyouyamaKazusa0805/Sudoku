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
	private IList<SettingGroupItem> _settingGroupItems;


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage()
	{
		InitializeComponent();

		InitializeSettingGroupItems();
	}


	/// <summary>
	/// Initializes the field <see cref="_settingGroupItems"/>.
	/// </summary>
	/// <seealso cref="_settingGroupItems"/>
	[MemberNotNull(nameof(_settingGroupItems))]
	private void InitializeSettingGroupItems()
		=> _settingGroupItems = new SettingGroupItem[]
		{
			new(
				R["SettingsPage_GroupItemName_Basic"]!,
				R["SettingsPage_GroupItemDescription_Basic"]!,
				new SettingItem[]
				{
					new ToggleSwitchSettingItem(
						R["SettingsPage_ItemName_ShowCandidates"]!,
						nameof(Preference.ShowCandidates)
					),
					new ToggleSwitchSettingItem(
						R["SettingsPage_ItemName_ShowCandidateBorderLines"]!,
						nameof(Preference.ShowCandidateBorderLines)
					),
					new ToggleSwitchSettingItem(
						R["SettingsPage_ItemName_EnableDeltaValuesDisplaying"]!,
						R["SettingsPage_ItemDescription_EnableDeltaValuesDisplaying"]!,
						nameof(Preference.EnableDeltaValuesDisplaying)
					),
					new FontPickerSettingItem(
						R["SettingsPage_ItemName_ValueFontScale"]!,
						R["SettingsPage_ItemDescription_ValueFontScale"]!,
						nameof(Preference.ValueFontName)
					)
					{
						FontScalePropertyName = nameof(Preference.ValueFontScale)
					},
					new FontPickerSettingItem(
						R["SettingsPage_ItemName_CandidateFontScale"]!,
						R["SettingsPage_ItemDescription_CandidateFontScale"]!,
						nameof(Preference.CandidateFontName)
					)
					{
						FontScalePropertyName = nameof(Preference.CandidateFontScale)
					}
				}
			),
			new(R["SettingsPage_GroupItemName_Solving"]!, R["SettingsPage_GroupItemDescription_Solving"]!),
			new(
				R["SettingsPage_GroupItemName_Rendering"]!,
				R["SettingsPage_GroupItemDescription_Rendering"]!,
				new SettingItem[]
				{
					new SliderSettingItem(
						R["SettingsPage_ItemName_OutsideBorderWidth"]!,
						R["SettingsPage_ItemDescription_OutsideBorderWidth"]!,
						nameof(Preference.OutsideBorderWidth),
						stepFrequency: .1,
						tickFrequency: .3,
						minValue: 0,
						maxValue: 3
					),
					new SliderSettingItem(
						R["SettingsPage_ItemName_BlockBorderWidth"]!,
						nameof(Preference.BlockBorderWidth),
						stepFrequency: .5,
						tickFrequency: .5,
						minValue: 0,
						maxValue: 5
					),
					new SliderSettingItem(
						R["SettingsPage_ItemName_CellBorderWidth"]!,
						nameof(Preference.CellBorderWidth),
						stepFrequency: .5,
						tickFrequency: .5,
						minValue: 0,
						maxValue: 5
					),
					new SliderSettingItem(
						R["SettingsPage_ItemName_CandidateBorderWidth"]!,
						R["SettingsPage_ItemDescription_CandidateBorderWidth"]!,
						nameof(Preference.CandidateBorderWidth),
						stepFrequency: .1,
						tickFrequency: .3,
						minValue: 0,
						maxValue: 3
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_OutsideBorderColor"]!,
						nameof(Preference.OutsideBorderColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_BlockBorderColor"]!,
						nameof(Preference.BlockBorderColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_CellBorderColor"]!,
						nameof(Preference.CellBorderColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_CandidateBorderColor"]!,
						R["SettingsPage_ItemDescription_CandidateBorderColor"]!,
						nameof(Preference.CandidateBorderColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_GivenColor"]!,
						nameof(Preference.GivenColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_ModifiableColor"]!,
						nameof(Preference.ModifiableColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_CandidateColor"]!,
						nameof(Preference.CandidateColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_CellDeltaColor"]!,
						R["SettingsPage_ItemDescription_CellDeltaColor"]!,
						nameof(Preference.CellDeltaColor)
					),
					new ColorPickerSettingItem(
						R["SettingsPage_ItemName_CandidateDeltaColor"]!,
						R["SettingsPage_ItemDescription_CandidateDeltaColor"]!,
						nameof(Preference.CandidateDeltaColor)
					)
				}
			),
			new(
				R["SettingsPage_GroupItemName_Miscellaneous"]!,
				R["SettingsPage_GroupItemDescription_Miscellaneous"]!,
				new[]
				{
					new ToggleSwitchSettingItem(
						R["SettingsPage_ItemName_DescendingOrderedInfoBarBoard"]!,
						nameof(Preference.DescendingOrderedInfoBarBoard)
					)
				}
			)
		};

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
				new JsonSerializerOptions
				{
					WriteIndented = true,
					IgnoreReadOnlyProperties = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				}
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
