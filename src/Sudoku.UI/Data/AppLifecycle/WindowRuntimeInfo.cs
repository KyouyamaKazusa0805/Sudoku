namespace Sudoku.UI.Data.AppLifecycle;

/// <summary>
/// Defines the information to control the runtime page information.
/// </summary>
internal sealed class WindowRuntimeInfo
{
	/// <summary>
	/// Indicates the main window.
	/// </summary>
	public MainWindow MainWindow { get; internal set; } = null!;

	/// <summary>
	/// Indicates the user preference used.
	/// </summary>
	public Preference UserPreference { get; } = new();

	/// <summary>
	/// Indicates the possible preference items.
	/// </summary>
	public IList<SettingGroupItem> PreferenceItemGroups { get; internal set; } = null!;
}
