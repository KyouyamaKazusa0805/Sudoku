namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines the instance that can dynamically creates an instance of type <typeparamref name="TSettingItem"/>.
/// </summary>
/// <typeparam name="TSettingItem">The type of the setting item.</typeparam>
public interface IDynamicCreatableItem<out TSettingItem>
	where TSettingItem : SettingItem, IDynamicCreatableItem<TSettingItem>
{
	/// <summary>
	/// Creates a <see cref="SettingItem"/> instance dynamically.
	/// </summary>
	/// <param name="propertyName">
	/// The property name that references a property in the type <see cref="Preference"/>.
	/// </param>
	/// <returns>A <see cref="SettingItem"/> instance.</returns>
	public static abstract TSettingItem CreateInstance(string propertyName);
}
