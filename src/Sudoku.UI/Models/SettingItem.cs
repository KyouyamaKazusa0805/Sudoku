namespace Sudoku.UI.Models;

/// <summary>
/// Defines a pair of value that defines a setting item.
/// </summary>
public sealed class SettingItem
{
	/// <summary>
	/// Initializes a <see cref="SettingItem"/> instance via the properties.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SettingItem(string name, string preferenceValueName)
		=> (Name, PreferenceValueName) = (name, preferenceValueName);

	/// <summary>
	/// Initializes a <see cref="SettingItem"/> instance via the properties.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SettingItem(string name, string description, string preferenceValueName)
		=> (Name, Description, PreferenceValueName) = (name, description, preferenceValueName);


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
	/// Indicates the name corresponding to the current name. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string PreferenceValueName { get; set; } = string.Empty;


	/// <summary>
	/// Try to get the preference value from the current instance.
	/// </summary>
	/// <returns>The value of the preference.</returns>
	public bool GetPreference()
	{
		var instance = ((App)Application.Current).UserPreference;
		return (bool)typeof(UserPreference).GetField(PreferenceValueName)!.GetValue(instance)!;
	}

	/// <summary>
	/// Try to set the preference value to the current instance.
	/// </summary>
	/// <param name="value">The value.</param>
	public void SetPreference(bool value)
	{
		var instance = ((App)Application.Current).UserPreference;
		typeof(UserPreference).GetField(PreferenceValueName)!.SetValue(instance, value);
	}
}
