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
	public static abstract TSettingItem DynamicCreate(string propertyName);


	/// <summary>
	/// Gets attribute arguments of a property via the property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The attribute arguments of a property.</returns>
	/// <exception cref="InvalidOperationException">Throws when the specified property cannot be found.</exception>
	protected internal static sealed PreferenceAttribute<TSettingItem>? GetAttributeArguments(string propertyName)
	{
		var pi = typeof(Preference).GetProperty(propertyName) ?? throw new InvalidOperationException();
		var attributeType = typeof(PreferenceAttribute<>).MakeGenericType(new[] { typeof(TSettingItem) });
		var attribute = pi.GetCustomAttribute(attributeType);
		return (PreferenceAttribute<TSettingItem>?)attribute;
	}

	/// <summary>
	/// Gets the setting preference name of the item via the property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The name of the preference.</returns>
	protected internal static sealed string GetItemNameString(string propertyName)
		// Leading underscore characters '_' are intentional.
		// The character suggests the option is reserved by author himself.
		// We should ignore the extra underscores.
		=> R[$"SettingsPage_ItemName_{propertyName.TrimStart('_')}"]!;

	/// <summary>
	/// Gets the setting preference description of the item via the property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The description of the preference.</returns>
	protected internal static sealed string? GetItemDescriptionString(string propertyName)
		// Leading underscore characters '_' are intentional.
		// The character suggests the option is reserved by author himself.
		// We should ignore the extra underscores.
		=> R[$"SettingsPage_ItemDescription_{propertyName.TrimStart('_')}"];
}
