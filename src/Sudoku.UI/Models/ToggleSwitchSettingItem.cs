namespace Sudoku.UI.Models;

/// <summary>
/// Defines a boolean option that is binding with a toggle switch on displaying.
/// </summary>
public sealed class ToggleSwitchSettingItem : SettingItem
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ToggleSwitchSettingItem(string name, string preferenceValueName) : base(name, preferenceValueName)
	{
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ToggleSwitchSettingItem(string name, string description, string preferenceValueName) :
		base(name, description, preferenceValueName)
	{
	}


	/// <summary>
	/// Indicates the on content displayed.
	/// </summary>
	public string OnContent { get; set; } = R["ToggleSwitchDefaultOnContent"]!;

	/// <summary>
	/// Indicates the off content displayed.
	/// </summary>
	public string OffContent { get; set; } = R["ToggleSwitchDefaultOffContent"]!;


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public bool GetPreference() => GetPreference<bool>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(bool value) => SetPreference<bool>(value);
}
