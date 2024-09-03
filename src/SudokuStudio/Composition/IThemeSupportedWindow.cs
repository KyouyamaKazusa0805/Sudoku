namespace SudokuStudio.Composition;

/// <summary>
/// Represents a <see cref="Window"/> instance that supports theme.
/// </summary>
/// <seealso cref="Window"/>
internal interface IThemeSupportedWindow
{
	/// <summary>
	/// Set title bar button colors using the specified theme.
	/// </summary>
	/// <param name="theme">The theme.</param>
	public abstract void ManuallySetTitleBarButtonsColor(Theme theme);


	/// <summary>
	/// Sets the theme.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="theme">The theme to be set.</param>
	public static sealed void SetTheme(IThemeSupportedWindow window, Theme theme)
	{
		window.ManuallySetTitleBarButtonsColor(theme);

		if (window is Window { Content: FrameworkElement control })
		{
			control.RequestedTheme = theme switch
			{
				Theme.Light => ElementTheme.Light,
				Theme.Dark => ElementTheme.Dark,
				_ => App.ShouldSystemUseDarkMode() ? ElementTheme.Dark : ElementTheme.Light
			};

			if (window is IBackdropSupportedWindow backdropWindow)
			{
				var backdrop = Application.Current.AsApp().Preference.UIPreferences.Backdrop;
				IBackdropSupportedWindow.ManuallyUpdateBackground(backdropWindow, backdrop);
			}
		}
	}
}
