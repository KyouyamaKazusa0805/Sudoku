namespace Sudoku.UI.Models;

/// <summary>
/// Defines a setting group item.
/// </summary>
public sealed class SettingGroupItem
{
	/// <summary>
	/// Indicates the name of the setting item.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the description of the setting item.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// Indicates the setting items in the group.
	/// </summary>
	public IList<SettingItem?> SettingItem { get; set; } = new List<SettingItem?> { null };
}
