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


	private void GivenFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontName = e;

	private void GivenFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontScale = e;

	private void GivenFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontColor = e;

	private void ModifiableFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontName = e;

	private void ModifiableFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontScale = e;

	private void ModifiableFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor = e;

	private void PencilmarkFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontName = e;

	private void PencilmarkFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontScale = e;

	private void PencilmarkFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor = e;

	private void CoordinateFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontName = e;

	private void CoordinateFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontScale = e;

	private void CoordinateFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor = e;

	private void BabaGroupingFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontName = e;

	private void BabaGroupingFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontScale = e;

	private void BabaGroupingFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor = e;
}
