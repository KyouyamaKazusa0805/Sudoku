namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color option that is binding with a color picker on displaying.
/// </summary>
public sealed class ColorPickerSettingItem : SettingItem
{
	/// <inheritdoc/>
	public override ColorPickerSettingItem DynamicCreate(string propertyName)
		=> GetAttributeArguments(propertyName) switch
		{
			PreferenceAttribute<ColorPickerSettingItem> { Data: [] }
				=> new()
				{
					Name = GetItemNameString(propertyName)!,
					Description = GetItemDescriptionString(propertyName) ?? string.Empty,
					PreferenceValueName = propertyName
				},
			_ => throw new InvalidOperationException()
		};

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public Color GetPreference() => GetPreference<Color>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(Color value) => SetPreference<Color>(value);
}
