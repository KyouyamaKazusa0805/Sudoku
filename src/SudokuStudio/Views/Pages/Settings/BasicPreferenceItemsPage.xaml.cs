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


	private void LanguageSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(LanguageSettingPage), true);

	private void ThemeSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(ThemeSettingPage), true);

	private void SudokuGridBehaviorsSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(SudokuGridBehaviorsSettingPage), true);

	private void NotationSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(NotationSettingPage), true);

	private void AnimationFeedbackSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(AnimationFeedbackSettingPage), true);

	private void HistorySettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(HistorySettingPage), true);

	private void MiscellaneousBehaviorsSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(MiscellaneousBasicSettingPage), true);
}
