namespace Sudoku.UI.Models;

/// <summary>
/// Defines a pair of value that defines a setting item.
/// </summary>
public sealed class SettingItem
{
	/// <summary>
	/// Indicates the name of the setting. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the description of the setting. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the item value used.
	/// </summary>
	public object ItemValue { get; set; } = null!;
}
