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
		var uiPref = Application.Current.AsApp().Preference.UIPreferences;
		ThemeComboBox.SelectedIndex = (int)uiPref.CurrentTheme;
	}

	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	private bool EnsureUnsnapped() => ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();


	private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var theme = (Theme)((SegmentedItem)ThemeComboBox.SelectedItem).Tag!;
		Application.Current.AsApp().Preference.UIPreferences.CurrentTheme = theme;

		// Manually set theme.
		foreach (var window in Application.Current.AsApp().WindowManager.ActiveWindows.OfType<MainWindow>())
		{
			WindowComposition.SetTheme(window, theme);
		}
	}

	private void BackdropSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: string s } } && Enum.TryParse<BackdropKind>(s, out var value))
		{
			Application.Current.AsApp().Preference.UIPreferences.Backdrop = value;
		}
	}

	private async void BackgroundPictureButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (!EnsureUnsnapped())
		{
			return;
		}

		var fop = new FileOpenPicker();
		fop.Initialize(this);
		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.AddFileFormat(FileFormats.PortablePicture);

		if (await fop.PickSingleFileAsync() is not { Path: var filePath })
		{
			return;
		}

		foreach (var window in Application.Current.AsApp().WindowManager.ActiveWindows.OfType<IBackgroundPictureSupportedWindow>())
		{
			WindowComposition.SetBackgroundPicture(window, filePath);
		}

		BackgroundPicturePathDisplayer.Text = filePath;
		Application.Current.AsApp().Preference.UIPreferences.BackgroundPicturePath = filePath;
	}
}
