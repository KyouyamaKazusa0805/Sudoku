namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	private int _selectedCell;

	private Visibility _coordinateLabelsVisibility;

	private Grid _puzzle;

	private readonly SudokuPaneCell[] _children = new SudokuPaneCell[81];


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane() => InitializeComponent();


	/// <summary>
	/// Indicates the selected cell.
	/// </summary>
	public int SelectedCell
	{
		get => _selectedCell;

		set
		{
			if (_selectedCell == value)
			{
				return;
			}

			_selectedCell = Clamp(value, -1, 80);

			PropertyChanged?.Invoke(this, new(nameof(SelectedCell)));
		}
	}

	/// <summary>
	/// Indicates the visibility of coordinate labels.
	/// </summary>
	public Visibility CoordinateLabelsVisibility
	{
		get => _coordinateLabelsVisibility;

		set
		{
			if (_coordinateLabelsVisibility == value)
			{
				return;
			}

			_coordinateLabelsVisibility = value;

			PropertyChanged?.Invoke(this, new(nameof(CoordinateLabelsVisibility)));
		}
	}

	/// <summary>
	/// Indicates the target grid puzzle.
	/// </summary>
	public Grid Puzzle
	{
		get => _puzzle;

		set
		{
			if (_puzzle == value)
			{
				return;
			}

			_puzzle = value;

			UpdateCellData(value);

			PropertyChanged?.Invoke(this, new(nameof(Puzzle)));
		}
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// To initialize <see cref="GridCellData"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void UpdateCellData(scoped in Grid grid)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = _children[i];
			cellControl.CandidatesMask = grid.GetCandidates(i);
			cellControl.CellStatus = grid.GetStatus(i);
		}
	}


	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = new SudokuPaneCell { CellIndex = i };

			GridLayout.SetRow(cellControl, i / 9 + 2);
			GridLayout.SetColumn(cellControl, i % 9 + 2);

			MainGrid.Children.Add(cellControl);
			_children[i] = cellControl;
		}

		for (var i = 0; i < 81; i++)
		{
			var cellControl = _children[i];
			cellControl.PointerEntered += pointerEnteredOrExitedEventHandler;
			cellControl.PointerExited += pointerEnteredOrExitedEventHandler;


			void pointerEnteredOrExitedEventHandler(object sender, PointerRoutedEventArgs e)
			{
				for (var i = 0; i < 81; i++)
				{
					if (_children[i].IsPointerEntered)
					{
						SelectedCell = i;
						return;
					}
				}
			}
		}
	}

	private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		var (_, height) = e.NewSize;
		if (height.NearlyEquals(0, 1E-3F))
		{
			return;
		}

		var valueFontSizeUnified = (height - (5 << 1)) / 10;

		foreach (var control in MainGrid.Children)
		{
			switch (control)
			{
				case TextBlock c:
				{
					c.FontSize = valueFontSizeUnified / 3;

					break;
				}
				case SudokuPaneCell c:
				{
					c.ValueFontSize = valueFontSizeUnified / 2;
					c.CandidateFontSize = valueFontSizeUnified / 3;

					break;
				}
			}
		}
	}
}
