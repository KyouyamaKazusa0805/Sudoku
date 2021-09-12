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
		CreateCandidateEllipses();
		CreateRegionBorders();
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
	/// Creates candidate ellipses.
	/// </summary>
	private void CreateCandidateEllipses()
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

	/// <summary>
	/// Creates region borders.
	/// </summary>
	private void CreateRegionBorders()
	{
		int region = 0;
		for (; region < 9; region++)
		{
			var border = new Border
			{
				BorderThickness = new(1.5),
				Visibility = Visibility.Collapsed,
				BorderBrush = new SolidColorBrush(Colors.Blue),
				Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255)),
				Style = (Style)UiResources.Current.HighlightRegionStyle
			};

			Grid.SetRow(border, region switch { 0 or 3 or 6 => 0, 1 or 4 or 7 => 9, 2 or 5 or 8 => 18 });
			Grid.SetColumn(border, region switch { 0 or 1 or 2 => 0, 3 or 4 or 5 => 9, 6 or 7 or 8 => 18 });
			Grid.SetRowSpan(border, 9);
			Grid.SetColumnSpan(border, 9);

			MainSudokuGrid.Children.Add(border);

			_highlightRegionPool[region] = border;
		}
		for (; region < 18; region++)
		{
			var border = new Border
			{
				BorderThickness = new(1.5),
				Visibility = Visibility.Collapsed,
				BorderBrush = new SolidColorBrush(Colors.Blue),
				Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255)),
				Style = (Style)UiResources.Current.HighlightRegionStyle
			};

			Grid.SetRow(border, region % 9 * 3);
			Grid.SetColumn(border, 0);
			Grid.SetColumnSpan(border, 9);

			MainSudokuGrid.Children.Add(border);

			_highlightRegionPool[region] = border;
		}
		for (; region < 27; region++)
		{
			var border = new Border
			{
				BorderThickness = new(1.5),
				Visibility = Visibility.Collapsed,
				BorderBrush = new SolidColorBrush(Colors.Blue),
				Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255)),
				Style = (Style)UiResources.Current.HighlightRegionStyle
			};

			Grid.SetRow(border, 0);
			Grid.SetRowSpan(border, 9);
			Grid.SetColumn(border, region % 9 * 3);

			MainSudokuGrid.Children.Add(border);

			_highlightRegionPool[region] = border;
		}
	}
}
