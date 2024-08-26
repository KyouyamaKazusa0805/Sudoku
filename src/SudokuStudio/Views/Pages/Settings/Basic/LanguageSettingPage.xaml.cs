namespace SudokuStudio.Views.Pages.Settings.Basic;

/// <summary>
/// Indicates the language setting page.
/// </summary>
public sealed partial class LanguageSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="LanguageSettingPage"/> instance.
	/// </summary>
	public LanguageSettingPage()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Initializes for control properties.
	/// </summary>
	private void InitializeControls()
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		LanguageComboBox.SelectedIndex = uiPref.Language switch { 0 => 0, 1033 => 1, 2052 => 2 };
	}


	private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> ((App)Application.Current).Preference.UIPreferences.Language = (int)((SegmentedItem)LanguageComboBox.SelectedItem).Tag!;
}
