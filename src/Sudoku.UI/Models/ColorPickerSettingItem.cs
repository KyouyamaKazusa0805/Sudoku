namespace Sudoku.UI.Models;

/// <summary>
/// Defines a color option that is binding with a color picker on displaying.
/// </summary>
public sealed class ColorPickerSettingItem : SettingItem
{
	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public Color GetPreference() => GetPreference<Color>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(Color value) => SetPreference<Color>(value);
}
