namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color array option that is binding with a group of color selectors on displaying.
/// </summary>
public sealed class ColorSelectorGroupSettingItem : SettingItem, IDynamicCreatableItem<ColorSelectorGroupSettingItem>
{
	/// <summary>
	/// Indicates the option contents.
	/// </summary>
	public required string[] OptionContents { get; set; }


	/// <inheritdoc/>
	public static ColorSelectorGroupSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<ColorSelectorGroupSettingItem>(propertyName) switch
		{
			{ Data: [(_, int count)] } => new ColorSelectorGroupSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName,
				OptionContents = (
					from value in Enumerable.Range(0, count)
					select R[$"SettingsPage_ItemName_{propertyName.TrimStart('_')}Option{value}Content"]!
				).ToArray()
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
	{
		var list = new List<ColorSelectorGroupInfo>();

		var colors = GetPreference();
		for (int i = 0; i < colors.Length; i++)
		{
			list.Add(
				new()
				{
					SettingItem = this,
					Index = i,
					Content = R[$"SettingsPage_ItemName_{PreferenceValueName.TrimStart('_')}Option{i}Content"]!,
					Color = colors[i]
				}
			);
		}

		return list;
	}

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
