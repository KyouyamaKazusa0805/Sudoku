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
}
