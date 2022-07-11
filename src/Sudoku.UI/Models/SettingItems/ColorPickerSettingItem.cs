namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color option that is binding with a color picker on displaying.
/// </summary>
public sealed class ColorPickerSettingItem : SettingItem, IDynamicCreatableItem<ColorPickerSettingItem>
{
	/// <inheritdoc/>
	public static ColorPickerSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<ColorPickerSettingItem>(propertyName) switch
		{
			{ Data: [] } => new ColorPickerSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName
			}
		};

		if (NamedValueLookup.GetItemDescriptionString(propertyName) is { } description)
		{
			result.Description = description;
		}

		return result;
	}


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public Color GetPreference() => GetPreference<Color>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(Color value) => SetPreference<Color>(value);
}
