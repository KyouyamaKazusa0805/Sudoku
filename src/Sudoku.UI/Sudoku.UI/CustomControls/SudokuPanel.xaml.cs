namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a basic sudoku panel.
/// </summary>
public sealed partial class SudokuPanel : UserControl
{
	/// <summary>
	/// The pool of candidate text blocks.
	/// </summary>
	private readonly TextBlock[] _candidateTextBlockPool = new TextBlock[729];

	/// <summary>
	/// The pool of highlight cells.
	/// </summary>
	private readonly Border[] _highlightCellPool = new Border[81];

	/// <summary>
	/// The pool of highlight candidates.
	/// </summary>
	private readonly Ellipse[] _highlightCandidatePool = new Ellipse[729];

	/// <summary>
	/// The pool of highlight regions.
	/// </summary>
	private readonly Border[] _highlightRegionPool = new Border[27];


	/// <summary>
	/// Initializes a <see cref="SudokuPanel"/> instance.
	/// </summary>
	public SudokuPanel()
	{
		InitializeComponent();

		CreateCandidateControls();
		CreateCellBorders();
		CreateCandidateBorders();
	}


	/// <summary>
	/// Creates candidates.
	/// </summary>
	private void CreateCandidateControls()
	{
		for (int candidate = 0; candidate < 729; candidate++)
		{
			int cell = candidate / 9, digit = candidate % 9;
			var tb = new TextBlock
			{
				Text = (digit + 1).ToString(),
				Visibility = Visibility.Visible,
				Foreground = new SolidColorBrush(Colors.Gray),
				FontSize = 12,
				FontFamily = new("Times New Roman"),
				Style = (Style)UiResources.Current.CandidateControlStyle
			};

			int row = cell / 9, column = cell % 9;

			Grid.SetRow(tb, row * 3 + digit / 3);
			Grid.SetColumn(tb, column * 3 + digit % 3);

			MainSudokuGrid.Children.Add(tb);

			_candidateTextBlockPool[cell * 9 + digit] = tb;
		}
	}

	/// <summary>
	/// Creates cell borders.
	/// </summary>
	private void CreateCellBorders()
	{
		for (int cell = 0; cell < 81; cell++)
		{
			var border = new Border
			{
				BorderThickness = new(1.5),
				Visibility = Visibility.Collapsed,
				BorderBrush = new SolidColorBrush(Colors.Blue),
				Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255)),
				Style = (Style)UiResources.Current.HighlightCellStyle
			};

			int row = cell / 9, column = cell % 9;
			Grid.SetRow(border, row * 3);
			Grid.SetRowSpan(border, 3);
			Grid.SetColumn(border, column * 3);
			Grid.SetColumnSpan(border, 3);

			MainSudokuGrid.Children.Add(border);

			_highlightCellPool[cell] = border;
		}
	}

	/// <summary>
	/// Creates candidate borders.
	/// </summary>
	private void CreateCandidateBorders()
	{
		for (int candidate = 0; candidate < 729; candidate++)
		{
			int cell = candidate / 9, digit = candidate % 9;
			var ellipse = new Ellipse
			{
				StrokeThickness = 1.5,
				Visibility = Visibility.Collapsed,
				Stroke = new SolidColorBrush(Colors.Blue),
				Fill = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255)),
				Style = (Style)UiResources.Current.HighlightCandidateStyle
			};

			int row = cell / 9, column = cell % 9;
			Grid.SetRow(ellipse, row / 3 + digit / 3);
			Grid.SetColumn(ellipse, column / 3 + digit % 3);

			MainSudokuGrid.Children.Add(ellipse);

			_highlightCandidatePool[candidate] = ellipse;
		}
	}
}
