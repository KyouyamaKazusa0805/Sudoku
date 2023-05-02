namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a font picker.
/// </summary>
[DependencyProperty<string>("SelectedFont")]
[DependencyProperty<decimal>("SelectedFontScale")]
[DependencyProperty<Color>("SelectedFontColor")]
public sealed partial class FontPicker : UserControl
{
	/// <summary>
	/// Indicates the <see cref="TextBlock"/> list that represents with fonts.
	/// </summary>
	private readonly IList<TextBlock> _fontTextBlocks =
		(
			from font in CanvasTextFormat.GetSystemFontFamilies()
			select new TextBlock { Text = font, FontFamily = new(font) }
		).ToList();


	/// <summary>
	/// Initializes a <see cref="FontPicker"/> instance.
	/// </summary>
	public FontPicker() => InitializeComponent();


	/// <summary>
	/// Indicates the event that will be triggered when the property <see cref="SelectedFont"/> has been changed.
	/// </summary>
	public event EventHandler<string>? SelectedFontChanged;

	/// <summary>
	/// Indicates the event that will be triggered when the property <see cref="SelectedFontScale"/> has been changed.
	/// </summary>
	public event EventHandler<decimal>? SelectedFontScaleChanged;

	/// <summary>
	/// Indicates the event that will be triggered when the property <see cref="SelectedFontColor"/> has been changed.
	/// </summary>
	public event EventHandler<Color>? SelectedFontColorChanged;


	private void SetSelectedFontScale(double value) => SelectedFontScale = (decimal)value;


	[Callback]
	private static void SelectedFontPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (FontPicker instance, { NewValue: string value }))
		{
			instance.SelectedFontChanged?.Invoke(instance, value);
		}
	}

	[Callback]
	private static void SelectedFontScalePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (FontPicker instance, { NewValue: decimal value }))
		{
			instance.SelectedFontScaleChanged?.Invoke(instance, value);
		}
	}

	[Callback]
	private static void SelectedFontColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (FontPicker instance, { NewValue: Color value }))
		{
			instance.SelectedFontColorChanged?.Invoke(instance, value);
		}
	}
}
