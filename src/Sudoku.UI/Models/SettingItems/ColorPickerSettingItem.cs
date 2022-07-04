namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color option that is binding with a color picker on displaying.
/// </summary>
public sealed class ColorPickerSettingItem : SettingItem, IDynamicCreatableItem<ColorPickerSettingItem>
{
	/// <inheritdoc/>
	public static ColorPickerSettingItem DynamicCreate(string propertyName)
		=> IDynamicCreatableItem<ColorPickerSettingItem>.GetAttributeArguments(propertyName) switch
		{
			{ Data: [] } => new()
			{
				Name = IDynamicCreatableItem<ColorPickerSettingItem>.GetItemNameString(propertyName),
				Description = IDynamicCreatableItem<ColorPickerSettingItem>.GetItemDescriptionString(propertyName) ?? string.Empty,
				PreferenceValueName = propertyName
			},
			_ => throw new InvalidOperationException()
		};


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public Color GetPreference() => GetPreference<Color>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(Color value) => SetPreference<Color>(value);
}
