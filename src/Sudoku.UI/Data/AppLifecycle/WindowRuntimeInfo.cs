namespace Sudoku.UI.Data.AppLifecycle;

/// <summary>
/// Defines the information to control the runtime page information.
/// </summary>
internal sealed class WindowRuntimeInfo
{
	/// <summary>
	/// Indicates the possible preference items.
	/// </summary>
	public IList<SettingGroupItem> Value { get; internal set; } = null!;
}
