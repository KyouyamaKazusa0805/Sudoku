namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a font picker control.
/// </summary>
public sealed partial class FontPicker : UserControl
{
	/// <summary>
	/// Initializes a <see cref="FontPicker"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FontPicker() => InitializeComponent();


	/// <inheritdoc cref="ComboBox.Header"/>
	public object Header
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cComboBox.Header;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _cComboBox.Header = value;
	}

	/// <inheritdoc cref="ComboBox.PlaceholderText"/>
	public string PlaceholderText
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cComboBox.PlaceholderText;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _cComboBox.PlaceholderText = value;
	}

	/// <summary>
	/// Indicates the chosen font name.
	/// </summary>
	public FontFamily FontNameChosen
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new((string)_cComboBox.SelectedValue);
	}

	/// <summary>
	/// Indicates the base setting item.
	/// </summary>
	public FontPickerSettingItem SettingItem { get; set; } = null!;
}
