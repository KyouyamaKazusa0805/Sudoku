namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Provides a setting item that binds with the enumeration type <see cref="PeerFocusingMode"/>.
/// </summary>
/// <seealso cref="PeerFocusingMode"/>
public sealed class PeerFocusingModeComboBoxSettingItem :
	ComboBoxSettingItem<PeerFocusingMode>,
	IDynamicCreatableItem<PeerFocusingModeComboBoxSettingItem>
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
	/// <exception cref="NotSupportedException">
	/// Throws when the specified type of the argument value is not supported.
	/// </exception>
	public static PeerFocusingModeComboBoxSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<PeerFocusingModeComboBoxSettingItem>(propertyName) switch
		{
			{ Data: [(_, var argument)] } => new PeerFocusingModeComboBoxSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName,
				OptionContents = argument switch
				{
					int count => getOptionContentsFromCount(count, propertyName),
					string[] values => getOptionContentsFromKey(values),
					_ => throw new NotSupportedException("The specified type of the argument value is not supported.")
				}
			},
			_ => throw new InvalidOperationException()
		};

		if (NamedValueLookup.GetItemDescriptionString(propertyName) is { } description)
		{
			result.Description = description;
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string[] getOptionContentsFromCount(int count, string propertyName)
			=> (
				from value in Enumerable.Range(0, count)
				select R[$"SettingsPage_ItemName_{propertyName.TrimStart('_')}Option{value}Content"]!
			).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string[] getOptionContentsFromKey(string[] keys) => (from key in keys select R[key]!).ToArray();
	}

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetPreferenceIndex() => (int)GetPreference();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPreferenceIndex(int value) => SetPreference((PeerFocusingMode)value);
}
