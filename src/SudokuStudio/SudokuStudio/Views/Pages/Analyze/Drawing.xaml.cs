namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Represents a tab page.
/// </summary>
public sealed partial class Drawing : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Initializes a <see cref="Drawing"/> instance.
	/// </summary>
	public Drawing() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc/>
	AnalyzerResult? IAnalyzeTabPage.AnalysisResult { get; set; }


	private void SetSelectedMode(int selectedIndex) => BasePage.SelectedMode = (DrawingMode)(selectedIndex + 1);

	private void SetLinkType(int selectedIndex)
		=> BasePage.LinkKind = Enum.Parse<Inference>((string)((ComboBoxItem)LinkTypeChoser.Items[selectedIndex]).Tag!);


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		foreach (var button in ColorPaletteButtonGroup.Children.OfType<Button>())
		{
			if (button.Tag is string s && int.TryParse(s, out var index))
			{
				button.Background = DrawingConversion.GetBrush(BasePage.UserDefinedPalette[index]);
			}
		}
	}

	private void ColorPaletteButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Button { Tag: string s } && int.TryParse(s, out var i))
		{
			BasePage.SelectedColorIndex = i;
		}
	}
}
