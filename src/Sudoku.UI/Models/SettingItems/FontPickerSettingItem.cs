namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color option that is binding with a font picker on displaying.
/// </summary>
public sealed class FontPickerSettingItem : SettingItem, IDynamicCreatableItem<FontPickerSettingItem>
{
	/// <summary>
	/// Initializes a <see cref="FontPickerSettingItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FontPickerSettingItem() => (Name, PreferenceValueName) = (null!, null!);


	/// <inheritdoc/>
	public static FontPickerSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<FontPickerSettingItem>(propertyName) switch
		{
			{ Data: [] data } => new FontPickerSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName
			},
			_ => throw new InvalidOperationException()
		};

		if (NamedValueLookup.GetItemDescriptionString(propertyName) is { } description)
		{
			result.Description = description;
		}

		return result;
	}

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public double GetFontScalePreference() => GetFontData().FontScale;

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public object GetFontNamePreference() => GetFontData().FontName;

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetFontScalePreference(double value) => SetFontData(GetFontData() with { FontScale = value });

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetFontNamePreference(object value) => SetFontData(GetFontData() with { FontName = (string)value });

	private FontData GetFontData()
	{
		var instance = ((App)Application.Current).UserPreference;
		return (FontData)typeof(Preference).GetProperty(PreferenceValueName)!.GetValue(instance)!;
	}

	private void SetFontData(FontData fontData)
	{
		var instance = ((App)Application.Current).UserPreference;
		typeof(Preference).GetProperty(PreferenceValueName)!.SetValue(instance, fontData);
	}
}
