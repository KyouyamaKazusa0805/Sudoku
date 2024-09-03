namespace SudokuStudio.Composition;

/// <summary>
/// Represents a <see cref="Window"/> instance that supports backdrop.
/// </summary>
/// <seealso cref="Window"/>
internal interface IBackdropSupportedWindow
{
	/// <summary>
	/// Indicates the root grid layout.
	/// </summary>
	public abstract Panel RootGridLayout { get; }


	/// <summary>
	/// Try to set Mica backdrop.
	/// </summary>
	/// <param name="useMicaAlt">Indicates whether the current Mica backdrop use alternated configuration.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the Mica backdrop is supported.</returns>
	public abstract bool TrySetMicaBackdrop(bool useMicaAlt);

	/// <summary>
	/// Try to set Acrylic backdrop.
	/// </summary>
	/// <param name="useAcrylicThin">Indicates whether the current Acrylic backdrop use thin configuration.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the Acrylic backdrop is supported.</returns>
	public abstract bool TrySetAcrylicBackdrop(bool useAcrylicThin);

	/// <summary>
	/// Try to dispose resource of backdrop-related resources.
	/// </summary>
	public abstract void DisposeBackdropRelatedResources();


	/// <summary>
	/// Manually set backdrop via the specified kind.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="backdrop">The backdrop to be set.</param>
	public static sealed void SetBackdrop(IBackdropSupportedWindow window, BackdropKind backdrop)
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
	internal static sealed void ManuallyUpdateBackground(IBackdropSupportedWindow window, BackdropKind backdrop)
	{
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
}
