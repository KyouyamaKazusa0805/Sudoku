namespace SudokuStudio.Views.Pages.Settings.Basic;

/// <summary>
/// Represents theme setting page.
/// </summary>
public sealed partial class ThemeSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="ThemeSettingPage"/> instance.
	/// </summary>
	public ThemeSettingPage()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Initializes for control properties.
	/// </summary>
	private void InitializeControls()
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		ThemeComboBox.SelectedIndex = (int)uiPref.CurrentTheme;
	}


	private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var theme = (Theme)((SegmentedItem)ThemeComboBox.SelectedItem).Tag!;
		((App)Application.Current).Preference.UIPreferences.CurrentTheme = theme;

		// Manually set theme.
		foreach (var window in ((App)Application.Current).WindowManager.ActiveWindows)
		{
			if (window is MainWindow instance)
			{
				instance.ManuallySetTitleBarButtonsColor(theme);
			}

			if (window.Content is FrameworkElement control)
			{
				control.RequestedTheme = theme switch
				{
					Theme.Default => ElementTheme.Default,
					Theme.Light => ElementTheme.Light,
					_ => ElementTheme.Dark
				};
			}
		}
	}
}
