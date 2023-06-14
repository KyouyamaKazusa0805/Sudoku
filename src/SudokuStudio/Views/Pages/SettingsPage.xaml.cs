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


	/// <summary>
	/// Gets the main window.
	/// </summary>
	/// <returns>The main window.</returns>
	/// <exception cref="InvalidOperationException">Throws when the base window cannot be found.</exception>
	private MainWindow GetMainWindow()
		=> ((App)Application.Current).WindowManager.GetWindowForElement(this) switch
		{
			MainWindow mainWindow => mainWindow,
			_ => throw new InvalidOperationException("Main window cannot be found.")
		};


	private void GoToBasicOptionsButton_Click(object sender, RoutedEventArgs e)
		=> GetMainWindow().NavigateToPage<BasicPreferenceItemsPage>();

	private void GoToAnalysisOptionsButton_Click(object sender, RoutedEventArgs e)
		=> GetMainWindow().NavigateToPage<AnalysisPreferenceItemsPage>();

	private void GoToRenderingOptionsButton_Click(object sender, RoutedEventArgs e)
		=> GetMainWindow().NavigateToPage<DrawingPreferenceItemsPage>();

	private async void OpenSettingsFolderButton_ClickAsync(object sender, RoutedEventArgs e)
		=> await Launcher.LaunchFolderPathAsync(SystemPath.GetDirectoryName(CommonPaths.UserPreference));
}
