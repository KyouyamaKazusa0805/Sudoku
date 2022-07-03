namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a color option that is binding with a font picker on displaying.
/// </summary>
public sealed class FontPickerSettingItem : SettingItem
{
	/// <summary>
	/// Initializes a <see cref="FontPickerSettingItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FontPickerSettingItem() => (Name, PreferenceValueName) = (null!, null!);


	/// <inheritdoc/>
	public override FontPickerSettingItem DynamicCreate(string propertyName)
		=> GetAttributeArguments(propertyName) switch
		{
			PreferenceAttribute<FontPickerSettingItem> { Data: [] data }
				=> new()
				{
					Name = GetItemNameString(propertyName)!,
					Description = GetItemDescriptionString(propertyName) ?? string.Empty,
					PreferenceValueName = propertyName
				},
			_ => throw new InvalidOperationException()
		};

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
