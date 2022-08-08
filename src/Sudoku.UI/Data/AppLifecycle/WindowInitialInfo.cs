namespace Sudoku.UI.Data.AppLifecycle;

/// <summary>
/// Defines the information to control the initial page information.
/// </summary>
internal sealed class WindowInitialInfo
{
	/// <summary>
	/// Indicates whether the window is created from the preference backup file.
	/// </summary>
	public bool FromPreferenceFile { get; internal set; } = false;

	/// <summary>
	/// Indicates the raw value of the drawing data. The default value is <see langword="null"/>.
	/// </summary>
	public string? DrawingDataRawValue { get; internal set; } = null;

	/// <summary>
	/// Indicates the first sudoku grid. The default value is <see langword="null"/>.
	/// </summary>
	public Grid? FirstGrid { get; internal set; } = null;


	/// <summary>
	/// Routes the current instance to the specified page type, returning the type's name.
	/// </summary>
	/// <returns>The type's name to be routed.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the current instance is invalid, or contains invalid data.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string RouteToPageName()
		=> this switch
		{
			{ FirstGrid: not null } => nameof(SudokuPage),
			{ DrawingDataRawValue: not null } => nameof(SudokuPage),
			{ FromPreferenceFile: true } => nameof(SettingsPage),
			_ => ((App)Application.Current).UserPreference.AlwaysShowHomePageWhenOpen switch
			{
				true => nameof(HomePage),
				_ => nameof(SudokuPage)
			}
		};
}
