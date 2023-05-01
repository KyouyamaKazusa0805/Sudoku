namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a font picker.
/// </summary>
[DependencyProperty<string>("SelectedFontName")]
[DependencyProperty<decimal>("SelectedFontScale")]
[DependencyProperty<Color>("SelectedColor")]
public sealed partial class FontPicker : UserControl
{
	[DefaultValue]
	private static readonly string SelectedFontNameDefaultValue = FontFamily.XamlAutoFontFamily.Source;

	[DefaultValue]
	private static readonly Color SelectedColorDefaultValue = Colors.Transparent;


	/// <summary>
	/// Indicates the fonts.
	/// </summary>
	private readonly IList<TextBlock> _fontsRange =
		(from font in CanvasTextFormat.GetSystemFontFamilies() select new TextBlock { Text = font, FontFamily = new(font) }).ToList();


	/// <summary>
	/// Initializes a <see cref="FontPicker"/> instance.
	/// </summary>
	public FontPicker() => InitializeComponent();


	/// <summary>
	/// Indicates the selected font data.
	/// </summary>
	public FontSerializationData SelectedFontData
		=> new() { FontName = SelectedFontName, FontScale = SelectedFontScale, FontColor = SelectedColor };


	private void SetSelectedFontScale(double value) => SelectedFontScale = (decimal)Round(value, 2);
}
