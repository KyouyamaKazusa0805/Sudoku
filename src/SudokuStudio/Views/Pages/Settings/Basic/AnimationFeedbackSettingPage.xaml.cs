namespace SudokuStudio.Views.Pages.Settings.Basic;

/// <summary>
/// Represents animation feedback setting page.
/// </summary>
public sealed partial class AnimationFeedbackSettingPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AnimationFeedbackSettingPage"/> instance.
	/// </summary>
	public AnimationFeedbackSettingPage() => InitializeComponent();


	private void HouseCompletedFeedbackColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.HouseCompletedFeedbackColor = e;
}
