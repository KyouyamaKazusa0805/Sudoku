namespace SudokuStudio.Views.Pages.Settings.Drawing;

/// <summary>
/// Represents delta setting page.
/// </summary>
public sealed partial class DeltaSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="DeltaSettingPage"/> instance.
	/// </summary>
	public DeltaSettingPage() => InitializeComponent();


	/// <summary>
	/// The delta value color.
	/// </summary>
	internal Color DeltaValueColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor,
			_ => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor_Dark = value
		};
	}

	/// <summary>
	/// The delta pencilmark color.
	/// </summary>
	internal Color DeltaPencilmarkColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor,
			_ => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor_Dark = value
		};
	}


	private void DeltaCellColorSelector_ColorChanged(object sender, Color e) => DeltaValueColor = e;

	private void DeltaCandidateColorSelector_ColorChanged(object sender, Color e) => DeltaPencilmarkColor = e;
}
