namespace Sudoku.UI.Models;

/// <summary>
/// Defines a boolean option that is binding with a toggle switch on displaying.
/// </summary>
public sealed class ToggleSwitchSettingItem : SettingItem
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ToggleSwitchSettingItem(string name, string preferenceValueName) : base(name, preferenceValueName)
	{
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ToggleSwitchSettingItem(string name, string description, string preferenceValueName) :
		base(name, description, preferenceValueName)
	{
	}


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
