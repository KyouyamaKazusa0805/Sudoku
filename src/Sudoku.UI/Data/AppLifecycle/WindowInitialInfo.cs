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
	/// Indicates the first page type name. The default value is <see langword="nameof"/>(<see cref="HomePage"/>).
	/// </summary>
	/// <seealso cref="HomePage"/>
	public string FirstPageTypeName { get; internal set; } = nameof(HomePage);

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the raw value of the drawing data. The default value is <see langword="null"/>.
	/// </summary>
	public string? DrawingDataRawValue { get; internal set; } = null;
#endif

	/// <summary>
	/// Indicates the first sudoku grid. The default value is <see langword="null"/>.
	/// </summary>
	public Grid? FirstGrid { get; internal set; } = null;

	/// <summary>
	/// Indicates the main window.
	/// </summary>
	public MainWindow MainWindow { get; internal set; } = null!;

	/// <summary>
	/// Indicates the user preference used.
	/// </summary>
	public Preference UserPreference { get; } = new();


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
#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
			{ DrawingDataRawValue: not null } => nameof(SudokuPage),
#endif
			{ FromPreferenceFile: true } => nameof(SettingsPage),
			{ FirstPageTypeName: var firstPageTypeName } => firstPageTypeName,
			_ => throw new InvalidOperationException("The initialization information is invalid.")
		};
}
