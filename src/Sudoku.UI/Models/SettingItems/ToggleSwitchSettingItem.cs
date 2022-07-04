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
	public static ToggleSwitchSettingItem DynamicCreate(string propertyName)
		=> IDynamicCreatableItem<ToggleSwitchSettingItem>.GetAttributeArguments(propertyName) switch
		{
			{ Data: { Length: <= 2 } data } => new()
			{
				Name = IDynamicCreatableItem<ToggleSwitchSettingItem>.GetItemNameString(propertyName),
				Description = IDynamicCreatableItem<ToggleSwitchSettingItem>.GetItemDescriptionString(propertyName) ?? string.Empty,
				PreferenceValueName = propertyName,
				OnContent = (string?)data.FirstOrDefault(static p => p.Key == nameof(OnContent)).Value
					?? R["ToggleSwitchDefaultOnContent"]!,
				OffContent = (string?)data.FirstOrDefault(static p => p.Key == nameof(OffContent)).Value
					?? R["ToggleSwitchDefaultOffContent"]!
			},
			_ => throw new InvalidOperationException()
		};

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public bool GetPreference() => GetPreference<bool>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(bool value) => SetPreference<bool>(value);
}
