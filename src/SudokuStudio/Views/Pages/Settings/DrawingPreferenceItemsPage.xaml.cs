namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents drawing preferences page.
/// </summary>
public sealed partial class DrawingPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="DrawingPreferenceItemsPage"/> instance.
	/// </summary>
	public DrawingPreferenceItemsPage() => InitializeComponent();


	/// <summary>
	/// The theme description.
	/// </summary>
	internal string ThemeDescription
		=> string.Format(
			SR.Get("SettingsPage_CurrentlySelectedThemeIs", App.CurrentCulture),
			[
				SR.Get(
					App.CurrentTheme switch
					{
						ApplicationTheme.Light => "SettingsPage_LightThemeFullName",
						_ => "SettingsPage_DarkThemeFullName"
					},
					App.CurrentCulture
				),
				SR.Get(
					App.CurrentTheme switch
					{
						ApplicationTheme.Light => "SettingsPage_DarkThemeFullName",
						_ => "SettingsPage_LightThemeFullName"
					},
					App.CurrentCulture
				)
			]
		);


	private void DeltaSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(DeltaSettingPage), true);

	private void ItemColorSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(ItemColorSettingPage), true);

	private void StepColorSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(StepColorSettingPage), true);

	private void FontSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(FontSettingPage), true);

	private void DashStyleSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(DashStyleSettingPage), true);

	private void MiscellaneousSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(MiscellaneousDrawingSettingPage), true);
}
