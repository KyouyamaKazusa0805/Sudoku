namespace SudokuStudio.Views.Pages.Settings.Drawing;

/// <summary>
/// Represents dash style setting page.
/// </summary>
public sealed partial class DashStyleSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="DashStyleSettingPage"/> instance.
	/// </summary>
	public DashStyleSettingPage() => InitializeComponent();


	private void StrongLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> Application.Current.AsApp().Preference.UIPreferences.StrongLinkDashStyle = e;

	private void WeakLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> Application.Current.AsApp().Preference.UIPreferences.WeakLinkDashStyle = e;
}
