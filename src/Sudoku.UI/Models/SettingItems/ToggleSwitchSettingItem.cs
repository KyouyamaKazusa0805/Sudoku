namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a boolean option that is binding with a toggle switch on displaying.
/// </summary>
public sealed class ToggleSwitchSettingItem : SettingItem
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
	public override ToggleSwitchSettingItem DynamicCreate(string propertyName)
		=> GetAttributeArguments(propertyName) switch
		{
			PreferenceAttribute<ToggleSwitchSettingItem> { Data: { Length: <= 2 } data }
				=> new()
				{
					Name = GetItemNameString(propertyName)!,
					Description = GetItemDescriptionString(propertyName) ?? string.Empty,
					PreferenceValueName = propertyName,
					OnContent = (string)data.First(p => p.Key == nameof(OnContent)).Value!,
					OffContent = (string)data.First(p => p.Key == nameof(OffContent)).Value!
				},
			_ => throw new InvalidOperationException()
		};

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public bool GetPreference() => GetPreference<bool>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(bool value) => SetPreference<bool>(value);
}
