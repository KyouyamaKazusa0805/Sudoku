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


	/// <inheritdoc/>
	public override PeerFocusingModeComboBoxSettingItem DynamicCreate(string propertyName)
		=> GetAttributeArguments(propertyName) switch
		{
			PreferenceAttribute<PeerFocusingModeComboBoxSettingItem> { Data: [(_, string[] values)] }
				=> new()
				{
					Name = GetItemNameString(propertyName)!,
					Description = GetItemDescriptionString(propertyName) ?? string.Empty,
					PreferenceValueName = propertyName,
					OptionContents = (from value in values select R[value]!).ToArray()
				},
			_ => throw new InvalidOperationException()
		};

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetPreferenceIndex() => (int)GetPreference();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPreferenceIndex(int value) => SetPreference((PeerFocusingMode)value);
}
