namespace Sudoku.UI.Models;

/// <summary>
/// Defines a combo box group setting item for enumeration type <see cref="PeerFocusingMode"/>.
/// </summary>
/// <seealso cref="PeerFocusingMode"/>
public sealed class PeerFocusingModeComboBoxGroupSettingItem : SettingItem
{
	/// <summary>
	/// Initializes a <see cref="PeerFocusingModeComboBoxGroupSettingItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PeerFocusingModeComboBoxGroupSettingItem()
		=> (Option0Content, Option1Content, Option2Content) = (null!, null!, null!);


	/// <summary>
	/// Indicates the option 0 content.
	/// </summary>
	public required string Option0Content { get; set; }

	/// <summary>
	/// Indicates the option 1 content.
	/// </summary>
	public required string Option1Content { get; set; }

	/// <summary>
	/// Indicates the option 2 content.
	/// </summary>
	public required string Option2Content { get; set; }


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PeerFocusingMode GetPreference() => GetPreference<PeerFocusingMode>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPreference(PeerFocusingMode value) => SetPreference<PeerFocusingMode>(value);

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal int GetPreferenceIndex() => (int)GetPreference<PeerFocusingMode>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void SetPreferenceIndex(int value) => SetPreference((PeerFocusingMode)value);
}
