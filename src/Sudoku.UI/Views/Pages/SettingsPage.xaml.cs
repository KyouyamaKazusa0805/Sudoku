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
		=> _settingGroupItems = new List<SettingGroupItem>
		{
			new()
			{
				Name = Get("SettingsPage_GroupItemName_Basic"),
				Description = Get("SettingsPage_GroupItemDescription_Basic"),
				SettingItem = new List<SettingItem?>
				{
					new(Get("SettingsPage_ItemName_ShowCandidates"), nameof(UserPreference.ShowCandidates)),
					new(
						Get("SettingsPage_ItemName_ShowCandidateBorderLines"),
						nameof(UserPreference.ShowCandidateBorderLines)
					),
					new(
						Get("SettingsPage_ItemName_EnableDeltaValuesDisplaying"),
						Get("SettingsPage_ItemDescription_EnableDeltaValuesDisplaying"),
						nameof(UserPreference.EnableDeltaValuesDisplaying)
					)
				}
			},
			new()
			{
				Name = Get("SettingsPage_GroupItemName_Solving"),
				Description = Get("SettingsPage_GroupItemDescription_Solving")
			},
			new()
			{
				Name = Get("SettingsPage_GroupItemName_Miscellaneous"),
				Description = Get("SettingsPage_GroupItemDescription_Miscellaneous"),
				SettingItem = new List<SettingItem?>
				{
					new(
						Get("SettingsPage_ItemName_DescendingOrderedInfoBarBoard"),
						nameof(UserPreference.DescendingOrderedInfoBarBoard)
					)
				}
			}
		};
}
