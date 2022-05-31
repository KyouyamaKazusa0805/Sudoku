namespace Sudoku.UI.Models;

/// <summary>
/// Defines a color option that is binding with a font picker on displaying.
/// </summary>
public sealed class FontPickerSettingItem : SettingItem
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FontPickerSettingItem(string name, string preferenceValueName) : base(name, preferenceValueName)
	{
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FontPickerSettingItem(string name, string description, string preferenceValueName) :
		base(name, description, preferenceValueName)
	{
	}


	/// <summary>
	/// Indicates the font name property name.
	/// </summary>
	public string FontScalePropertyName { get; set; } = null!;


	/// <inheritdoc cref="SettingItem.GetPreference{T}"/>
	public double GetFontScalePreference()
	{
		var instance = ((App)Application.Current).UserPreference;
		return (double)typeof(UserPreference).GetField(FontScalePropertyName)!.GetValue(instance)!;
	}

	/// <inheritdoc cref="SettingItem.GetPreference{T}"/>
	public string GetFontNamePreference() => GetPreference<string>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetFontScalePreference(double value)
	{
		var instance = ((App)Application.Current).UserPreference;
		typeof(UserPreference).GetField(FontScalePropertyName)!.SetValue(instance, value);
	}

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetFontNamePreference(string value) => SetPreference(value);
}
