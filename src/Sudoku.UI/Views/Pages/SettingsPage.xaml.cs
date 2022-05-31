namespace Sudoku.UI.Views.Pages;

/// <summary>
/// A page that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
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
				Get("SettingsPage_GroupItemName_Basic"),
				Get("SettingsPage_GroupItemDescription_Basic"),
				new SettingItem[]
				{
					new ToggleSwitchSettingItem(
						Get("SettingsPage_ItemName_ShowCandidates"),
						nameof(UserPreference.ShowCandidates)
					),
					new ToggleSwitchSettingItem(
						Get("SettingsPage_ItemName_ShowCandidateBorderLines"),
						nameof(UserPreference.ShowCandidateBorderLines)
					),
					new ToggleSwitchSettingItem(
						Get("SettingsPage_ItemName_EnableDeltaValuesDisplaying"),
						Get("SettingsPage_ItemDescription_EnableDeltaValuesDisplaying"),
						nameof(UserPreference.EnableDeltaValuesDisplaying)
					),
					new FontPickerSettingItem(
						Get("SettingsPage_ItemName_ValueFontScale"),
						Get("SettingsPage_ItemDescription_ValueFontScale"),
						nameof(UserPreference.ValueFontName)
					)
					{
						FontScalePropertyName = nameof(UserPreference.ValueFontScale)
					},
					new FontPickerSettingItem(
						Get("SettingsPage_ItemName_CandidateFontScale"),
						Get("SettingsPage_ItemDescription_CandidateFontScale"),
						nameof(UserPreference.CandidateFontName)
					)
					{
						FontScalePropertyName = nameof(UserPreference.CandidateFontScale)
					}
				}
			),
			new(Get("SettingsPage_GroupItemName_Solving"), Get("SettingsPage_GroupItemDescription_Solving")),
			new(
				Get("SettingsPage_GroupItemName_Rendering"),
				Get("SettingsPage_GroupItemDescription_Rendering"),
				new SettingItem[]
				{
					new SliderSettingItem(
						Get("SettingsPage_ItemName_OutsideBorderWidth"),
						Get("SettingsPage_ItemDescription_OutsideBorderWidth"),
						nameof(UserPreference.OutsideBorderWidth),
						stepFrequency: .1,
						tickFrequency: .3,
						minValue: 0,
						maxValue: 3
					),
					new SliderSettingItem(
						Get("SettingsPage_ItemName_BlockBorderWidth"),
						nameof(UserPreference.BlockBorderWidth),
						stepFrequency: .5,
						tickFrequency: .5,
						minValue: 0,
						maxValue: 5
					),
					new SliderSettingItem(
						Get("SettingsPage_ItemName_CellBorderWidth"),
						nameof(UserPreference.CellBorderWidth),
						stepFrequency: .5,
						tickFrequency: .5,
						minValue: 0,
						maxValue: 5
					),
					new SliderSettingItem(
						Get("SettingsPage_ItemName_CandidateBorderWidth"),
						Get("SettingsPage_ItemDescription_CandidateBorderWidth"),
						nameof(UserPreference.CandidateBorderWidth),
						stepFrequency: .1,
						tickFrequency: .3,
						minValue: 0,
						maxValue: 3
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_OutsideBorderColor"),
						nameof(UserPreference.OutsideBorderColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_BlockBorderColor"),
						nameof(UserPreference.BlockBorderColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_CellBorderColor"),
						nameof(UserPreference.CellBorderColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_CandidateBorderColor"),
						Get("SettingsPage_ItemDescription_CandidateBorderColor"),
						nameof(UserPreference.CandidateBorderColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_GivenColor"),
						nameof(UserPreference.GivenColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_ModifiableColor"),
						nameof(UserPreference.ModifiableColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_CandidateColor"),
						nameof(UserPreference.CandidateColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_CellDeltaColor"),
						Get("SettingsPage_ItemDescription_CellDeltaColor"),
						nameof(UserPreference.CellDeltaColor)
					),
					new ColorPickerSettingItem(
						Get("SettingsPage_ItemName_CandidateDeltaColor"),
						Get("SettingsPage_ItemDescription_CandidateDeltaColor"),
						nameof(UserPreference.CandidateDeltaColor)
					)
				}
			),
			new(
				Get("SettingsPage_GroupItemName_Miscellaneous"),
				Get("SettingsPage_GroupItemDescription_Miscellaneous"),
				new[]
				{
					new ToggleSwitchSettingItem(
						Get("SettingsPage_ItemName_DescendingOrderedInfoBarBoard"),
						nameof(UserPreference.DescendingOrderedInfoBarBoard)
					)
				}
			)
		};
}
