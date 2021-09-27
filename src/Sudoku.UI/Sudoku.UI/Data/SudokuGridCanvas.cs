namespace Sudoku.UI.Data;

/// <summary>
/// Provides the canvas that stores the sudoku grid details.
/// </summary>
/// <param name="Preference">
/// Indicates the <see cref="Data.Preference"/> instance that stores the base settings.
/// </param>
/// <param name="BaseGrid">
/// The <see cref="Grid"/> instance that stores and displays the <see cref="Shape"/>s.
/// </param>
/// <param name="CandidateTextBlockPool">Indicates the pool of candidate <see cref="TextBlock"/>s.</param>
/// <param name="CellTextBlockPool">Indicates the pool of cell <see cref="TextBlock"/>s.</param>
/// <param name="HighlightCellPool">
/// Indicates the pool of highlight cells displaying via <see cref="Border"/>s.
/// </param>
/// <param name="HighlightCandidatePool">
/// Indicates the pool of highlight candidates displaying via <see cref="Ellipse"/>s.
/// </param>
/// <param name="HighlightRegionPool">
/// Indicates the pool of highlight regions displaying via <see cref="Border"/>s.
/// </param>
public sealed record class SudokuGridCanvas(
	Preference Preference,
	Grid BaseGrid,
	TextBlock[] CellTextBlockPool,
	TextBlock[] CandidateTextBlockPool,
	Border[] HighlightCellPool,
	Ellipse[] HighlightCandidatePool,
	Border[] HighlightRegionPool
)
{
	/// <summary>
	/// Indicates the default text size.
	/// </summary>
	private const int DefaultTextSize = 60;


	/// <summary>
	/// Indicates the solver that checks the uniqueness of sudoku puzzles.
	/// </summary>
	private static readonly FastSolver Solver = new();


	/// <summary>
	/// Initializes a <see cref="SudokuGridCanvas"/> instance with the specified <see cref="Grid"/> instance
	/// indicating the base control stores those values.
	/// </summary>
	/// <param name="preference">Indicates the <see cref="Data.Preference"/> instance.</param>
	/// <param name="baseGrid">The base <see cref="Grid"/> instance.</param>
	private SudokuGridCanvas(Preference preference, Grid baseGrid)
	: this(
		preference, baseGrid, new TextBlock[81], new TextBlock[729],
		new Border[81], new Ellipse[729], new Border[27]
	)
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
				int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1
			)
			{
				var border = new Border
				{
					BorderThickness = new(left, top, right, bottom),
					BorderBrush = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.GridLineColorLight
							: Preference.GridLineColorDark
					)
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
				var tb = new TextBlock
				{
#if AUTHOR_RESERVED
					// Unmeaningful initialization... :D
					Text = (cell % 9 + 1).ToString(),
#endif
					Visibility = Visibility.Collapsed,
					Foreground = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.GivenColorLight
							: Preference.GivenColorDark
					),
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

				CellTextBlockPool[cell] = tb;
			}
		}

		void createCandidateControls()
		{
			for (int candidate = 0; candidate < 729; candidate++)
			{
				int cell = candidate / 9, digit = candidate % 9;
				var tb = new TextBlock
				{
#if AUTHOR_RESERVED
					// Unmeaningful initialization.
					Text = (digit + 1).ToString(),
#endif
					Visibility = Visibility.Visible,
					Foreground = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.CandidateColorLight
							: Preference.CandidateColorDark
					),
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

				CandidateTextBlockPool[cell * 9 + digit] = tb;
			}
		}

		void createCellBorders()
		{
			for (int cell = 0; cell < 81; cell++)
			{
				var border = new Border
				{
					BorderThickness = new(Preference.CellBorderWidth),
					Visibility = Visibility.Collapsed,
					BorderBrush = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.CellBorderColorLight
							: Preference.CellBorderColorDark
					),
					Background = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.CellBorderBackgroundColorLight
							: Preference.CellBorderBackgroundColorDark
					)
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

		void createCandidateEllipses()
		{
			for (int candidate = 0; candidate < 729; candidate++)
			{
				int cell = candidate / 9, digit = candidate % 9;
				var ellipse = new Ellipse
				{
					StrokeThickness = Preference.CandidateBorderWidth,
					Visibility = Visibility.Collapsed,
					Stroke = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.CandidateBorderColorLight
							: Preference.CandidateBorderColorDark
					),
					Fill = new SolidColorBrush(
						UiResources.LightOrDarkMode == ApplicationTheme.Light
							? Preference.CandidateBorderBackgroundColorLight
							: Preference.CandidateBorderBackgroundColorDark
					)
				};

				int row = cell / 9, column = cell % 9;
				Grid.SetRow(ellipse, row / 3 + digit / 3);
				Grid.SetColumn(ellipse, column / 3 + digit % 3);

				BaseGrid.Children.Add(ellipse);

				HighlightCandidatePool[candidate] = ellipse;
			}
		}

		void createRegionBorders()
		{
			int[] blockRowFactor = { 0, 9, 18, 0, 9, 18, 0, 9, 18 };
			int[] blockColumnFactor = { 0, 0, 0, 9, 9, 9, 18, 18, 18 };
			double uniformBorderThickness = Preference.RegionBorderWidth;
			var borderBrush = new SolidColorBrush(
				UiResources.LightOrDarkMode == ApplicationTheme.Light
					? Preference.RegionBorderColorLight
					: Preference.RegionBorderColorDark
			);
			var background = new SolidColorBrush(
				UiResources.LightOrDarkMode == ApplicationTheme.Light
					? Preference.RegionBorderBackgroundColorLight
					: Preference.RegionBorderBackgroundColorDark
			);

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

					var border = new Border
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

					HighlightRegionPool[region] = border;
				}
			}
		}
	}


	/// <summary>
	/// Load the sudoku puzzle.
	/// </summary>
	/// <param name="sudoku">The sudoku.</param>
	/// <exception cref="InvalidPuzzleException">Throws when the load opertion is failed.</exception>
	public void LoadSudoku(in SudokuGrid sudoku)
	{
		if (
			sudoku switch
			{
#if DEBUG
				{ IsDebuggerUndefined: true } => (string)UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_DebuggerUndefinedFailed1,
#endif
				{ IsUndefined: true } => (string)UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_UndefinedFailed,
				_ => Solver.CheckValidity($"{sudoku:0}") ? null : (string)UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_UniquenessFailed
			} is { } errorInfo
		)
		{
			throw new InvalidPuzzleException(sudoku, errorInfo);
		}

		RefreshCandidates(sudoku);
	}

	/// <summary>
	/// To refresh given and candidate controls via the <see cref="SudokuGrid"/> instance.
	/// </summary>
	/// <param name="sudoku">The base sudoku grid.</param>
	private void RefreshCandidates(in SudokuGrid sudoku)
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
						CandidateTextBlockPool[cell * 9 + digit].Visibility = Visibility.Visible;
					}
					foreach (int digit in (short)(SudokuGrid.MaxCandidatesMask & ~candidates))
					{
						CandidateTextBlockPool[cell * 9 + digit].Visibility = Visibility.Collapsed;
					}

					// Hide the cell text block.
					CellTextBlockPool[cell].Visibility = Visibility.Collapsed;

					break;
				}
				case CellStatus.Modifiable:
				{
					// Show the cell text block.
					CellTextBlockPool[cell].Foreground = new SolidColorBrush(Colors.LightGray);

					// Hide the candidate text block.
					for (int digit = 0; digit < 9; digit++)
					{
						CandidateTextBlockPool[cell * 9 + digit].Visibility = Visibility.Collapsed;
					}

					goto ModifiableOrGiven;
				}
				case CellStatus.Given:
				{
					// Show the cell text block.
					CellTextBlockPool[cell].Foreground = new SolidColorBrush(Colors.White);

					// Hide the candidate text block.
					for (int digit = 0; digit < 9; digit++)
					{
						CandidateTextBlockPool[cell * 9 + digit].Visibility = Visibility.Collapsed;
					}

					goto ModifiableOrGiven;
				}

			ModifiableOrGiven:
				{
					ref var element = ref CellTextBlockPool[cell];
					element.Visibility = Visibility.Visible;
					element.Text = (sudoku[cell] + 1).ToString();

					break;
				}
			}
		}
	}


	/// <summary>
	/// Creates the <see cref="SudokuGridCanvas"/> instance using the specified <see cref="Data.Preference"/> 
	/// and <see cref="Grid"/> instance asynchronuous.
	/// </summary>
	/// <param name="preference">The <see cref="Data.Preference"/> instance.</param>
	/// <param name="baseGrid">The base <see cref="Grid"/> instance.</param>
	/// <returns>A task that returns the <see cref="SudokuGridCanvas"/> instance.</returns>
	/// <remarks>
	/// Please note that the return value is of type <see cref="ValueTask{TResult}"/>,
	/// which means you can <b>only</b> <see langword="await"/> in caller one time.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTask<SudokuGridCanvas> CreateAsync(Preference preference, Grid baseGrid) =>
		ValueTask.FromResult<SudokuGridCanvas>(new(preference, baseGrid));
}
