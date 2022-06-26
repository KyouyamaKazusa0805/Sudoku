namespace Sudoku.UI.Models;

/// <summary>
/// Defines a combo box setting item.
/// </summary>
/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
public abstract class ComboBoxSettingItem<TEnum> : SettingItem where TEnum : unmanaged, Enum
{
	/// <summary>
	/// Initializes a <see cref="ComboBoxSettingItem{TEnum}"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ComboBoxSettingItem() => OptionContents = null!;


	/// <summary>
	/// Indicates the option contents.
	/// </summary>
	public required string[] OptionContents { get; set; }


	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected TEnum GetPreference() => GetPreference<TEnum>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void SetPreference(TEnum value) => SetPreference<TEnum>(value);
}
