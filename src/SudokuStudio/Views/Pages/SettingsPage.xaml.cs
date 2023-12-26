namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines the settings page.
/// </summary>
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage() => InitializeComponent();


	private void GoToBasicOptionsButton_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<BasicPreferenceItemsPage>();

	private void GoToLibraryOptionsButton_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<LibraryPreferenceItemsPage>();

	private void GoToAnalysisOptionsButton_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<AnalysisPreferenceItemsPage>();

	private void GoToRenderingOptionsButton_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<DrawingPreferenceItemsPage>();

	private async void OpenSettingsFolderButton_ClickAsync(object sender, RoutedEventArgs e)
		=> await Launcher.LaunchFolderPathAsync(io::Path.GetDirectoryName(CommonPaths.UserPreference));
}
