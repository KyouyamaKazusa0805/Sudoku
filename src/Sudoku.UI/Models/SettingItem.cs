namespace Sudoku.UI.Models;

/// <summary>
/// Defines an instance that defines a setting item.
/// </summary>
public abstract class SettingItem
{
	/// <summary>
	/// Initializes a <see cref="SettingItem"/> instance via the properties.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected SettingItem(string name, string preferenceValueName) :
		this(name, string.Empty, preferenceValueName)
	{
	}

	/// <summary>
	/// Initializes a <see cref="SettingItem"/> instance via the properties.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected SettingItem(string name, string description, string preferenceValueName)
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
	protected T GetPreference<T>()
	{
		var instance = ((App)Application.Current).UserPreference;
		return (T)typeof(UserPreference).GetField(PreferenceValueName)!.GetValue(instance)!;
	}

	/// <summary>
	/// Try to set the preference value to the current instance.
	/// </summary>
	/// <param name="value">The value.</param>
	protected void SetPreference<T>(T value)
	{
		var instance = ((App)Application.Current).UserPreference;
		typeof(UserPreference).GetField(PreferenceValueName)!.SetValue(instance, value);
	}
}
