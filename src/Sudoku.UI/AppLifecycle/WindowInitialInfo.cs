namespace Sudoku.UI.AppLifecycle;

/// <summary>
/// Defines the information to control the initial page information.
/// </summary>
internal sealed class WindowInitialInfo
{
	/// <summary>
	/// Indicates the first page type name. The default value is <see langword="nameof"/>(<see cref="HomePage"/>).
	/// </summary>
	/// <seealso cref="HomePage"/>
	public string FirstPageTypeName { get; internal set; } = nameof(HomePage);

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
}
