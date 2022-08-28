namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a font picker control.
/// </summary>
public sealed partial class FontPicker : UserControl
{
	/// <summary>
	/// The dependency property that binds with <see cref="SelectedFontName"/>.
	/// </summary>
	/// <seealso cref="SelectedFontName"/>
	public static readonly DependencyProperty SelectedFontNameProperty =
		DependencyProperty.Register(
			nameof(SelectedFontName),
			typeof(string),
			typeof(FontPicker),
			new(null)
		);

	/// <summary>
	/// The dependency property that binds with <see cref="SelectedFontScale"/>.
	/// </summary>
	/// <seealso cref="SelectedFontScale"/>
	public static readonly DependencyProperty SelectedFontScaleProperty =
		DependencyProperty.Register(
			nameof(SelectedFontScale),
			typeof(double),
			typeof(FontPicker),
			new(0.0)
		);


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

	/// <summary>
	/// Indicates the selected font scale.
	/// </summary>
	public double SelectedFontScale
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (double)GetValue(SelectedFontScaleProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(SelectedFontScaleProperty, value);
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
	/// Indicates the selected font name.
	/// </summary>
	public string SelectedFontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(SelectedFontNameProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(SelectedFontNameProperty, value);
	}
}
