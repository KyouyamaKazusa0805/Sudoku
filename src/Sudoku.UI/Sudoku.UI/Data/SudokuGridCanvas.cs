namespace Sudoku.UI.Data;

/// <summary>
/// Provides the canvas that stores the sudoku grid details.
/// </summary>
/// <param name="BaseGrid">
/// The <see cref="Grid"/> instance that stores and displays the <see cref="Shape"/>s.
/// </param>
/// <param name="CandidateTextBlockPool">The pool of candidate text blocks.</param>
/// <param name="HighlightCellPool">The pool of highlight cells.</param>
/// <param name="HighlightCandidatePool">The pool of highlight candidates.</param>
/// <param name="HighlightRegionPool">The pool of highlight regions.</param>
public sealed record SudokuGridCanvas(
	Grid BaseGrid,
	TextBlock[] CandidateTextBlockPool,
	Border[] HighlightCellPool,
	Ellipse[] HighlightCandidatePool,
	Border[] HighlightRegionPool
)
{
	/// <summary>
	/// Initializes a <see cref="SudokuGridCanvas"/> instance with the specified <see cref="Grid"/> instance
	/// indicating the base control stores those values.
	/// </summary>
	/// <param name="baseGrid">The base <see cref="Grid"/> instance.</param>
	public SudokuGridCanvas(Grid baseGrid)
	: this(baseGrid, new TextBlock[729], new Border[81], new Ellipse[729], new Border[27])
	{
		CreateMainGridOutlines();

		CreateCandidateControls();
		CreateCellBorders();
		CreateCandidateEllipses();
		CreateRegionBorders();
	}


	/// <summary>
	/// Generates the outlines of the sudoku grid.
	/// </summary>
	private void CreateMainGridOutlines()
	{
		const int size = 27;
		for (int i = 0; i < size; i++)
		{
			BaseGrid.RowDefinitions.Add(new());
			BaseGrid.ColumnDefinitions.Add(new());
		}

		for (int i = 0; i <= size; i += 3)
		{
			switch (i)
			{
				case 0:
				case 9:
				case 18:
				{
					f(top: 3, row: i, columnSpan: size);
					f(left: 3, column: i, rowSpan: size);

					break;
				}
				case 27:
				{
					f(bottom: 3, row: 26, columnSpan: size);
					f(right: 3, column: 26, rowSpan: size);

					break;
				}
				default:
				{
					f(top: 1, row: i, columnSpan: size);
					f(left: 1, column: i, rowSpan: size);

					break;
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void f(
			double left = 0, double top = 0, double right = 0, double bottom = 0,
			int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1
		)
		{
			var border = new Border
			{
				BorderThickness = new(left, top, right, bottom),
				Style = (Style)UiResources.Current.SudokuGridOutlineStyle
			};

			Grid.SetRow(border, row);
			Grid.SetRowSpan(border, rowSpan);
			Grid.SetColumn(border, column);
			Grid.SetColumnSpan(border, columnSpan);

			BaseGrid.Children.Add(border);
		}
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

			BaseGrid.Children.Add(tb);

			CandidateTextBlockPool[cell * 9 + digit] = tb;
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

			BaseGrid.Children.Add(border);

			HighlightCellPool[cell] = border;
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

			BaseGrid.Children.Add(ellipse);

			HighlightCandidatePool[candidate] = ellipse;
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

			BaseGrid.Children.Add(border);

			HighlightRegionPool[region] = border;
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

			BaseGrid.Children.Add(border);

			HighlightRegionPool[region] = border;
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

			BaseGrid.Children.Add(border);

			HighlightRegionPool[region] = border;
		}
	}
}
