namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a font picker.
/// </summary>
public sealed partial class FontPicker : UserControl
{
	/// <summary>
	/// The dependency property that binds with <see cref="SelectedFontName"/>.
	/// </summary>
	/// <seealso cref="SelectedFontName"/>
	public static readonly DependencyProperty SelectedFontNameProperty =
		DependencyProperty.Register(nameof(SelectedFontName), typeof(string), typeof(FontPicker), new(FontFamily.XamlAutoFontFamily.Source));

	/// <summary>
	/// The dependency property that binds with <see cref="SelectedFontScale"/>.
	/// </summary>
	/// <seealso cref="SelectedFontScale"/>
	public static readonly DependencyProperty SelectedFontScaleProperty =
		DependencyProperty.Register(nameof(SelectedFontScale), typeof(double), typeof(FontPicker), new(0D));

	/// <summary>
	/// The dependency property that binds with <see cref="SelectedColor"/>.
	/// </summary>
	/// <seealso cref="SelectedColor"/>
	public static readonly DependencyProperty SelectedColorProperty =
		DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(FontPicker), new(Colors.Transparent));


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
	/// Indicates the selected font scale.
	/// </summary>
	public double SelectedFontScale
	{
		get => (double)GetValue(SelectedFontScaleProperty);

		set => SetValue(SelectedFontScaleProperty, value);
	}

	/// <summary>
	/// Indicates the selected font name.
	/// </summary>
	public string SelectedFontName
	{
		get => (string)GetValue(SelectedFontNameProperty);

		set => SetValue(SelectedFontNameProperty, value);
	}

	/// <summary>
	/// Indicates the selected font name.
	/// </summary>
	public Color SelectedColor
	{
		get => (Color)GetValue(SelectedColorProperty);

		set => SetValue(SelectedColorProperty, value);
	}

	/// <summary>
	/// Indicates the selected font data.
	/// </summary>
	public FontSerializationData SelectedFontData
		=> new() { FontName = SelectedFontName, FontScale = SelectedFontScale, FontColor = SelectedColor };
}
