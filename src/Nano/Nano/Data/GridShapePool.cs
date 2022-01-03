using Circle = Microsoft.UI.Xaml.Shapes.Ellipse;
using Line = Microsoft.UI.Xaml.Controls.Border;
using Text = Microsoft.UI.Xaml.Controls.TextBlock;

namespace Nano;

/// <summary>
/// Defines a pool that stores the basic <see cref="Shape"/>s.
/// </summary>
/// <seealso cref="Shape"/>
/// <param name="Preference">Indicates the preferences.</param>
/// <param name="BaseGrid">
/// The <see cref="Grid"/> instance that stores and displays the <see cref="Shape"/>s.
/// </param>
/// <param name="CandidateTexts">Indicates the pool of candidate <see cref="Text"/>s.</param>
/// <param name="CellTexts">Indicates the pool of cell <see cref="Text"/>s.</param>
/// <param name="HighlightCells">
/// Indicates the pool of highlight cells displaying via <see cref="Line"/>s.
/// </param>
/// <param name="HighlightCandidates">
/// Indicates the pool of highlight candidates displaying via <see cref="Circle"/>s.
/// </param>
/// <param name="HighlightRegions">
/// Indicates the pool of highlight regions displaying via <see cref="Line"/>s.
/// </param>
public sealed record GridShapePool(
	Preference Preference, Grid BaseGrid, Text[] CellTexts, Text[] CandidateTexts,
	Line[] HighlightCells, Circle[] HighlightCandidates, Line[] HighlightRegions)
{
	private const int DefaultTextSize = 60;


	/// <summary>
	/// Initializes a <see cref="GridShapePool"/> instance with the specified
	/// <see cref="Grid"/> instance indicating the base control stores those values.
	/// </summary>
	/// <param name="preference">Indicates the preferences.</param>
	/// <param name="baseGrid">The base <see cref="Grid"/> instance.</param>
	public GridShapePool(Preference preference, Grid baseGrid)
	: this(preference, baseGrid, new Text[81], new Text[729], new Line[81], new Circle[729], new Line[27])
	{
		createMainGridOutlines();
		createCellControls();
		createCandidateControls();
		createCellBorders();
		createCandidateEllipses();
		createRegionBorders();


		void createMainGridOutlines()
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
						f(top: Preference.BlockLineWidth, row: i, columnSpan: size);
						f(left: Preference.BlockLineWidth, column: i, rowSpan: size);

						break;
					}
					case 27:
					{
						f(bottom: Preference.BlockLineWidth, row: 26, columnSpan: size);
						f(right: Preference.BlockLineWidth, column: 26, rowSpan: size);

						break;
					}
					default:
					{
						f(top: Preference.GridLineWidth, row: i, columnSpan: size);
						f(left: Preference.GridLineWidth, column: i, rowSpan: size);

						break;
					}
				}
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void f(
				double left = 0, double top = 0, double right = 0, double bottom = 0,
				int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1)
			{
				var border = new Line
				{
					BorderThickness = new(left, top, right, bottom),
					BorderBrush = new SolidColorBrush(Preference.GridLineColor)
				};

				Grid.SetRow(border, row);
				Grid.SetRowSpan(border, rowSpan);
				Grid.SetColumn(border, column);
				Grid.SetColumnSpan(border, columnSpan);

				BaseGrid.Children.Add(border);
			}
		}

		void createCellControls()
		{
			for (int cell = 0; cell < 81; cell++)
			{
				var tb = new Text
				{
					Visibility = Visibility.Collapsed,
					Foreground = new SolidColorBrush(Preference.GivenColor),
					FontSize = (double)(DefaultTextSize * Preference.ValueScale),
					FontFamily = new(Preference.GivenFontName),
					FontStyle = Preference.GivenFontStyle,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalTextAlignment = TextAlignment.Center
				};

				int row = cell / 9, column = cell % 9;
				Grid.SetRow(tb, row * 3);
				Grid.SetRowSpan(tb, 3);
				Grid.SetColumn(tb, column * 3);
				Grid.SetColumnSpan(tb, 3);

				BaseGrid.Children.Add(tb);

				CellTexts[cell] = tb;
			}
		}

		void createCandidateControls()
		{
			for (int candidate = 0; candidate < 729; candidate++)
			{
				int cell = candidate / 9, digit = candidate % 9;
				var tb = new Text
				{
					Visibility = Visibility.Visible,
					Foreground = new SolidColorBrush(Preference.CandidateColor),
					FontSize = (double)(DefaultTextSize * (Preference.CandidateScale / 3M)),
					FontFamily = new(Preference.CandidateFontName),
					FontStyle = Preference.CandidateFontStyle,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalTextAlignment = TextAlignment.Center
				};

				int row = cell / 9, column = cell % 9;
				Grid.SetRow(tb, row * 3 + digit / 3);
				Grid.SetColumn(tb, column * 3 + digit % 3);

				BaseGrid.Children.Add(tb);

				CandidateTexts[cell * 9 + digit] = tb;
			}
		}

		void createCellBorders()
		{
			for (int cell = 0; cell < 81; cell++)
			{
				var border = new Line
				{
					BorderThickness = new(Preference.CellBorderWidth),
					Visibility = Visibility.Collapsed,
					BorderBrush = new SolidColorBrush(Preference.CellBorderColor),
					Background = new SolidColorBrush(Preference.CellBorderBackgroundColor)
				};

				int row = cell / 9, column = cell % 9;
				Grid.SetRow(border, row * 3);
				Grid.SetRowSpan(border, 3);
				Grid.SetColumn(border, column * 3);
				Grid.SetColumnSpan(border, 3);

				BaseGrid.Children.Add(border);

				HighlightCells[cell] = border;
			}
		}

		void createCandidateEllipses()
		{
			for (int candidate = 0; candidate < 729; candidate++)
			{
				int cell = candidate / 9, digit = candidate % 9;
				var circle = new Circle
				{
					StrokeThickness = Preference.CandidateBorderWidth,
					Visibility = Visibility.Collapsed,
					Stroke = new SolidColorBrush(Preference.CandidateBorderColor),
					Fill = new SolidColorBrush(Preference.CandidateBorderBackgroundColor)
				};

				int row = cell / 9, column = cell % 9;
				Grid.SetRow(circle, row / 3 + digit / 3);
				Grid.SetColumn(circle, column / 3 + digit % 3);

				BaseGrid.Children.Add(circle);

				HighlightCandidates[candidate] = circle;
			}
		}

		void createRegionBorders()
		{
			int[] blockRowFactor = { 0, 9, 18, 0, 9, 18, 0, 9, 18 };
			int[] blockColumnFactor = { 0, 0, 0, 9, 9, 9, 18, 18, 18 };
			double uniformBorderThickness = Preference.RegionBorderWidth;
			var borderBrush = new SolidColorBrush(Preference.RegionBorderColor);
			var background = new SolidColorBrush(Preference.RegionBorderBackgroundColor);

			f(0, uniformBorderThickness, borderBrush, background);
			f(9, uniformBorderThickness, borderBrush, background);
			f(18, uniformBorderThickness, borderBrush, background);


			void f(int start, double uniformLength, Brush borderBrush, Brush background)
			{
				for (int region = start; region < start + 9; region++)
				{
					var (row, column, rowSpan, columnSpan) = start switch
					{
						0 => (blockRowFactor[region], blockColumnFactor[region], 9, 9),
						9 => (region % 9 * 3, 0, 1, 9),
						18 => (0, region % 9 * 3, 9, 1)
					};

					var border = new Line
					{
						BorderThickness = new(uniformLength),
						Visibility = Visibility.Collapsed,
						BorderBrush = borderBrush,
						Background = background
					};

					Grid.SetRow(border, row);
					Grid.SetColumn(border, column);
					Grid.SetRowSpan(border, rowSpan);
					Grid.SetColumnSpan(border, columnSpan);

					BaseGrid.Children.Add(border);

					HighlightRegions[region] = border;
				}
			}
		}
	}


	/// <summary>
	/// To refresh given and candidate controls via the <see cref="SudokuGrid"/> instance.
	/// </summary>
	/// <param name="sudoku">The base sudoku grid.</param>
	public void RefreshCandidates(in SudokuGrid sudoku)
	{
		for (int cell = 0; cell < 81; cell++)
		{
			switch (sudoku.GetStatus(cell))
			{
				case CellStatus.Empty:
				{
					// Show or hide the candidate text block.
					short candidates = sudoku.GetCandidates(cell);
					foreach (int digit in candidates)
					{
						CandidateTexts[cell * 9 + digit].Visibility = Visibility.Visible;
					}
					foreach (int digit in (short)(SudokuGrid.MaxCandidatesMask & ~candidates))
					{
						CandidateTexts[cell * 9 + digit].Visibility = Visibility.Collapsed;
					}

					// Hide the cell text block.
					CellTexts[cell].Visibility = Visibility.Collapsed;

					break;
				}
				case CellStatus.Modifiable:
				{
					// Show the cell text block.
					CellTexts[cell].Foreground = new SolidColorBrush(Colors.LightGray);

					// Hide the candidate text block.
					for (int digit = 0; digit < 9; digit++)
					{
						CandidateTexts[cell * 9 + digit].Visibility = Visibility.Collapsed;
					}

					goto ModifiableOrGiven;
				}
				case CellStatus.Given:
				{
					// Show the cell text block.
					CellTexts[cell].Foreground = new SolidColorBrush(Colors.White);

					// Hide the candidate text block.
					for (int digit = 0; digit < 9; digit++)
					{
						CandidateTexts[cell * 9 + digit].Visibility = Visibility.Collapsed;
					}

					goto ModifiableOrGiven;
				}

			ModifiableOrGiven:
				{
					ref var element = ref CellTexts[cell];
					element.Visibility = Visibility.Visible;
					element.Text = (sudoku[cell] + 1).ToString();

					break;
				}
			}
		}
	}
}
