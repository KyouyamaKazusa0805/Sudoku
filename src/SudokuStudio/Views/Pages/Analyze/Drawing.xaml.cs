namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Represents a tab page.
/// </summary>
public sealed partial class Drawing : Page, IAnalyzerTab
{
	/// <summary>
	/// Initializes a <see cref="Drawing"/> instance.
	/// </summary>
	public Drawing() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc/>
	AnalysisResult? IAnalyzerTab.AnalysisResult { get; set; }


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

	private void ClearItems_Click(object sender, RoutedEventArgs e)
	{
		if (BasePage is not { SelectedMode: var mode and not 0, _userColoringView: { View: var view } localViewUnit } || !Enum.IsDefined(mode))
		{
			return;
		}

		view.RemoveWhere(
			node => mode switch
			{
				DrawingMode.Cell => node is CellViewNode,
				DrawingMode.Candidate => node is CandidateViewNode,
				DrawingMode.House => node is HouseViewNode,
				DrawingMode.Chute => node is ChuteViewNode,
				DrawingMode.BabaGrouping => node is BabaGroupViewNode,
				DrawingMode.Link => node is LinkViewNode
			}
		);

		BasePage.SudokuPane.ViewUnit = null;
		BasePage.SudokuPane.ViewUnit = localViewUnit;

		BasePage.SelectedColorIndex = -1;
	}

	private void ClearAllViewItems_Click(object sender, RoutedEventArgs e)
	{
		if (BasePage is not { SelectedMode: var mode and not 0, _userColoringView: { View: var view } localViewUnit } || !Enum.IsDefined(mode))
		{
			return;
		}

		view.Clear();

		BasePage.SudokuPane.ViewUnit = null;
		BasePage.SudokuPane.ViewUnit = localViewUnit;

		BasePage.SelectedColorIndex = -1;
	}
}
