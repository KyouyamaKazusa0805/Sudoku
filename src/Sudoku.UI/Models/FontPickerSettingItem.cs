namespace Sudoku.UI.Models;

/// <summary>
/// Defines a color option that is binding with a font picker on displaying.
/// </summary>
public sealed class FontPickerSettingItem : SettingItem
{
	/// <summary>
	/// Initializes a <see cref="FontPickerSettingItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	public FontPickerSettingItem() => FontScalePropertyName = null!;


	/// <summary>
	/// Indicates the font name property name.
	/// </summary>
	public required string FontScalePropertyName { get; set; }


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public double GetFontScalePreference() => GetPreference<double>(FontScalePropertyName);

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public object GetFontNamePreference() => GetPreference<object>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetFontScalePreference(double value) => SetPreference(value, FontScalePropertyName);

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetFontNamePreference(object value) => SetPreference(value);
}
