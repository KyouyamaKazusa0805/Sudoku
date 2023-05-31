namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents a settings page that displays for basic preferences.
/// </summary>
public sealed partial class BasicPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="BasicPreferenceItemsPage"/> instance.
	/// </summary>
	public BasicPreferenceItemsPage() => InitializeComponent();


	private void BackdropSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is not ComboBox { SelectedItem: ComboBoxItem { Tag: string s } } || !Enum.TryParse<BackdropKind>(s, out var value))
		{
			return;
		}

		((App)Application.Current).Preference.UIPreferences.Backdrop = value;
	}
}
