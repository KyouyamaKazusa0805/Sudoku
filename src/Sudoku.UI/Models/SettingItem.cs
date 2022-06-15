namespace Sudoku.UI.Models;

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
}
