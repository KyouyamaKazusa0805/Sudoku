namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines an instance that defines a setting item.
/// </summary>
public abstract class SettingItem
{
	/// <summary>
	/// Indicates the name of the setting.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the description of the setting. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the name corresponding to the current name.
	/// </summary>
	public required string PreferenceValueName { get; set; }


	/// <summary>
	/// Creates a <see cref="SettingItem"/> instance dynamically.
	/// </summary>
	/// <param name="propertyName">
	/// The property name that references a property in the type <see cref="Preference"/>.
	/// </param>
	/// <returns>A <see cref="SettingItem"/> instance.</returns>
	public abstract SettingItem DynamicCreate(string propertyName);

	/// <summary>
	/// Try to get the preference value from the current instance.
	/// </summary>
	/// <returns>The value of the preference.</returns>
	protected T GetPreference<T>()
	{
		var instance = ((App)Application.Current).UserPreference;
		return (T)typeof(Preference).GetProperty(PreferenceValueName)!.GetValue(instance)!;
	}

	/// <summary>
	/// Try to get the preference value from the current instance.
	/// </summary>
	/// <param name="name">The name of the property should be checked.</param>
	/// <returns>The value of the preference.</returns>
	protected T GetPreference<T>(string name)
	{
		var instance = ((App)Application.Current).UserPreference;
		return (T)typeof(Preference).GetProperty(name)!.GetValue(instance)!;
	}

	/// <summary>
	/// Try to set the preference value to the current instance.
	/// </summary>
	/// <param name="value">The value.</param>
	protected void SetPreference<T>(T value)
	{
		var instance = ((App)Application.Current).UserPreference;
		typeof(Preference).GetProperty(PreferenceValueName)!.SetValue(instance, value);
	}

	/// <summary>
	/// Try to set the preference value to the current instance.
	/// </summary>
	/// <param name="name">The name of the property should be checked.</param>
	/// <param name="value">The value.</param>
	protected void SetPreference<T>(T value, string name)
	{
		var instance = ((App)Application.Current).UserPreference;
		typeof(Preference).GetProperty(name)!.SetValue(instance, value);
	}

	/// <summary>
	/// Gets attribute arguments of a property via the property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The attribute arguments of a property.</returns>
	/// <exception cref="InvalidOperationException">Throws when the specified property cannot be found.</exception>
	private protected Attribute? GetAttributeArguments(string propertyName)
	{
		var pi = typeof(Preference).GetProperty(propertyName) ?? throw new InvalidOperationException();
		var attributeType = typeof(PreferenceAttribute<>).MakeGenericType(new[] { GetType() });
		var attribute = pi.GetCustomAttribute(attributeType);
		return attribute;
	}


	private protected static string? GetItemNameString(string propertyName)
		=> R[$"SettingsPage_ItemName_{propertyName}"];

	private protected static string? GetItemDescriptionString(string propertyName)
		=> R[$"SettingsPage_ItemDescription_{propertyName}"];
}
