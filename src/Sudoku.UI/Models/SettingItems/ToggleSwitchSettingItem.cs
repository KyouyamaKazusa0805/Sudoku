namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a boolean option that is binding with a toggle switch on displaying.
/// </summary>
public sealed class ToggleSwitchSettingItem : SettingItem, IDynamicCreatableItem<ToggleSwitchSettingItem>
{
	/// <summary>
	/// Indicates the on content displayed.
	/// </summary>
	public string OnContent { get; set; } = R["ToggleSwitchDefaultOnContent"]!;

	/// <summary>
	/// Indicates the off content displayed.
	/// </summary>
	public string OffContent { get; set; } = R["ToggleSwitchDefaultOffContent"]!;


	/// <inheritdoc/>
	public static ToggleSwitchSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<ToggleSwitchSettingItem>(propertyName) switch
		{
			{ Data: { Length: <= 2 } data } => new ToggleSwitchSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName,
				OnContent = data.GetNamedValue(nameof(OnContent), R["ToggleSwitchDefaultOnContent"]!),
				OffContent = data.GetNamedValue(nameof(OffContent), R["ToggleSwitchDefaultOffContent"]!)
			}
		};

		if (NamedValueLookup.GetItemDescriptionString(propertyName) is { } description)
		{
			result.Description = description;
		}

		return result;
	}

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public bool GetPreference() => GetPreference<bool>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(bool value) => SetPreference<bool>(value);
}
