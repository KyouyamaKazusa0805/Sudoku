namespace SudokuStudio.Composition;

/// <summary>
/// Represents a list of methods that operates with <see cref="Window"/> objects, with composition options.
/// </summary>
/// <seealso cref="Window"/>
internal static class WindowComposition
{
	/// <summary>
	/// Manually set backdrop via the specified kind.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="backdrop">The backdrop to be set.</param>
	public static void SetBackdrop(IBackdropSupportedWindow window, BackdropKind backdrop)
	{
		switch (backdrop)
		{
			case BackdropKind.Default: { window.DisposeBackdropRelatedResources(); break; }
			case BackdropKind.Mica: { window.TrySetMicaBackdrop(false); break; }
			case BackdropKind.MicaDeep: { window.TrySetMicaBackdrop(true); break; }
			case BackdropKind.Acrylic: { window.TrySetAcrylicBackdrop(false); break; }
			case BackdropKind.AcrylicThin: { window.TrySetAcrylicBackdrop(true); break; }
			case BackdropKind.Transparent: { break; } // Not supported.
		}
		ManuallyUpdateBackground(window, backdrop);
	}

	/// <summary>
	/// Manually update background.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="backdrop">The backdrop.</param>
	public static void ManuallyUpdateBackground(IBackdropSupportedWindow window, BackdropKind backdrop)
	{
		if (window is IBackgroundPictureSupportedWindow { RootGridLayout: _ }
			&& Application.Current.AsApp().Preference.UIPreferences.BackgroundPicturePath is not null)
		{
			// This will override the configuration of the setting.
			return;
		}

		var isDark = Application.Current.AsApp().Preference.UIPreferences.CurrentTheme switch
		{
			Theme.Light => false,
			Theme.Dark => true,
			_ => App.ShouldSystemUseDarkMode()
		};
		if (isDark && backdrop == BackdropKind.Default && (isDark ? "Dark" : "Light") is var resourceDictionaryKey)
		{
			var dictionary = (ResourceDictionary)Application.Current.Resources.ThemeDictionaries[resourceDictionaryKey];
			window.RootGridLayout.Background = (Brush)dictionary["_DefaultBackground"]!;
			return;
		}

		window.RootGridLayout.Background = null;
	}

	/// <summary>
	/// Sets the theme.
	/// </summary>
	/// <typeparam name="TWindow">The type of the window.</typeparam>
	/// <param name="window">The window.</param>
	/// <param name="theme">The theme to be set.</param>
	public static void SetTheme<TWindow>(TWindow window, Theme theme)
		where TWindow : Window, IThemeSupportedWindow, IBackdropSupportedWindow
	{
		window.ManuallySetTitleBarButtonsColor(theme);

		if (window.Content is FrameworkElement control)
		{
			control.RequestedTheme = theme switch
			{
				Theme.Light => ElementTheme.Light,
				Theme.Dark => ElementTheme.Dark,
				_ => App.ShouldSystemUseDarkMode() ? ElementTheme.Dark : ElementTheme.Light
			};
			var backdrop = Application.Current.AsApp().Preference.UIPreferences.Backdrop;
			ManuallyUpdateBackground(window, backdrop);
		}
	}

	/// <summary>
	/// Sets the background picture.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="filePath">The background picture path.</param>
	public static void SetBackgroundPicture(IBackgroundPictureSupportedWindow window, string? filePath)
		=> window.RootGridLayout.Background = filePath is null
			? null
			: new ImageBrush { ImageSource = new BitmapImage(new(filePath)) };
}
