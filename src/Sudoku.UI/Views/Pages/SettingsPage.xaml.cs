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
	private readonly IList<SettingGroupItem> _settingGroupItems = new List<SettingGroupItem>
	{
		new() { Name = "Group 1", Description = "Description 1" },
		new() { Name = "Group 2", Description = "Description 2" },
		new() { Name = "Group 3", Description = "Description 3" }
	};


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage() => InitializeComponent();
}
