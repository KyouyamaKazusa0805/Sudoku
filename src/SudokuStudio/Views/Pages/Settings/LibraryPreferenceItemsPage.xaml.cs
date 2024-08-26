namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents a page that displays for preference items used in puzzle library page and its related pages.
/// </summary>
public sealed partial class LibraryPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="LibraryPreferenceItemsPage"/> instance.
	/// </summary>
	public LibraryPreferenceItemsPage() => InitializeComponent();


	private void LibrarySettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(LibrarySettingPage), true);
}
