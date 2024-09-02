namespace SudokuStudio.Views.Pages.Settings.Library;

/// <summary>
/// Represents library setting page.
/// </summary>
public sealed partial class LibrarySettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="LibrarySettingPage"/> instance.
	/// </summary>
	public LibrarySettingPage()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Initializes for controls.
	/// </summary>
	private void InitializeControls()
	{
		var libPref = Application.Current.AsApp().Preference.LibraryPreferences;
		CandidateDisplayingComboBox.SelectedIndex = (int)libPref.LibraryCandidatesVisibility;
	}


	private void CandidateDisplayingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> Application.Current.AsApp().Preference.LibraryPreferences.LibraryCandidatesVisibility = (LibraryCandidatesVisibility)CandidateDisplayingComboBox.SelectedIndex;
}
