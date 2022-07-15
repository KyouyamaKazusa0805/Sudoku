namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color array option that is binding with a group of color selectors on displaying.
/// </summary>
public sealed class ColorSelectorGroupSettingItem : SettingItem, IDynamicCreatableItem<ColorSelectorGroupSettingItem>
{
	/// <inheritdoc/>
	public static ColorSelectorGroupSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<ColorSelectorGroupSettingItem>(propertyName) switch
		{
			{ Data: [] } => new ColorSelectorGroupSettingItem
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
	public Color[] GetPreference() => GetPreference<Color[]>();

	/// <summary>
	/// Try to get the preference at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The color instance.</returns>
	public Color GetPreferenceForIndex(int index) => GetPreference()[index];

	/// <summary>
	/// To construct the info list.
	/// </summary>
	/// <returns>The info list that is used for binding.</returns>
	public IList<ColorSelectorGroupInfo> ConstructInfoList()
		=> GetPreference()
			.Select((color, i) => new ColorSelectorGroupInfo { SettingItem = this, Index = i, Color = color })
			.ToList();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(object value) => SetPreference((Color[])value);

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(Color[] value) => SetPreference<Color[]>(value);

	/// <summary>
	/// Try to set the color value at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <param name="color">The color.</param>
	public void SetPreferenceForIndex(int index, Color color) => GetPreference()[index] = color;
}
