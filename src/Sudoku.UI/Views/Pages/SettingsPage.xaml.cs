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
					new()
					{
						Name = "Nested item 1",
						Description = "Nested description 1",
						ItemValue = true
					},
					new()
					{
						Name = "Nested item 2",
						Description = "Nested description 2",
						ItemValue = true
					}
				}
			},
			new()
			{
				Name = Get("SettingsPage_GroupItemName_Solving"),
				Description = Get("SettingsPage_GroupItemDescription_Solving"),
				SettingItem = new List<SettingItem?> { null }
			},
			new()
			{
				Name = Get("SettingsPage_GroupItemName_Miscellaneous"),
				Description = Get("SettingsPage_GroupItemDescription_Miscellaneous"),
				SettingItem = new List<SettingItem?>
				{
					new()
					{
						Name = "Nested item 1",
						Description = "Nested description 1",
						ItemValue = true
					},
					new()
					{
						Name = "Nested item 2",
						Description = "Nested description 2",
						ItemValue = true
					},
					new()
					{
						Name = "Nested item 3",
						Description = "Nested description 3",
						ItemValue = true
					},
				}
			}
		};
}
