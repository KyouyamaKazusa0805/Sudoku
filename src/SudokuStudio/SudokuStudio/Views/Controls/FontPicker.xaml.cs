namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a font picker.
/// </summary>
[DependencyProperty<string>("SelectedFontName", DefaultValueGeneratingMemberName = nameof(SelectedFontNamePropertyDefaultValue))]
[DependencyProperty<double>("SelectedFontScale")]
[DependencyProperty<Color>("SelectedColor", DefaultValueGeneratingMemberName = nameof(SelectedColorPropertyDefaultValue))]
public sealed partial class FontPicker : UserControl
{
	private static readonly string SelectedFontNamePropertyDefaultValue = FontFamily.XamlAutoFontFamily.Source;
	private static readonly Color SelectedColorPropertyDefaultValue = Colors.Transparent;


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
}
