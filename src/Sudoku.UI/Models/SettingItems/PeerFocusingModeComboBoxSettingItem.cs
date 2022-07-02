namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Provides a setting item that binds with the enumeration type <see cref="PeerFocusingMode"/>.
/// </summary>
/// <seealso cref="PeerFocusingMode"/>
public sealed class PeerFocusingModeComboBoxSettingItem : ComboBoxSettingItem<PeerFocusingMode>
{
	/// <summary>
	/// Initializes a <see cref="PeerFocusingModeComboBoxSettingItem"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PeerFocusingModeComboBoxSettingItem() : base()
	{
	}


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetPreferenceIndex() => (int)GetPreference();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPreferenceIndex(int value) => SetPreference((PeerFocusingMode)value);
}
