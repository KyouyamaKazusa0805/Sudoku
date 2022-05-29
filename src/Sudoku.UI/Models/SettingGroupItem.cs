namespace Sudoku.UI.Models;

/// <summary>
/// Defines a setting group item.
/// </summary>
public sealed class SettingGroupItem
{
	/// <summary>
	/// Initializes a <see cref="SettingGroupItem"/> instance via the specified name and the specified description.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="description">The description.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SettingGroupItem(string name, string description) : this(name, description, new List<SettingItem?>() { null })
	{
	}

	/// <summary>
	/// Initializes a <see cref="SettingGroupItem"/> instance via the specified name, the specified description
	/// and the sub-setting items.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="description">The description.</param>
	/// <param name="settingItem">The setting items.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SettingGroupItem(string name, string description, IList<SettingItem?> settingItem)
		=> (Name, Description, SettingItem) = (name, description, settingItem);


	/// <summary>
	/// Indicates the name of the setting item. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the description of the setting item. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the setting items in the group.
	/// </summary>
	public IList<SettingItem?> SettingItem { get; set; } = new List<SettingItem?> { null };
}
