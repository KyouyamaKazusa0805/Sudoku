namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	private readonly SudokuPaneCell[] _children = new SudokuPaneCell[81];

	private int _selectedCell;

	private Visibility _coordinateLabelsVisibility;

	private Grid _puzzle;


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
			var valueFontSizeUnified = ((Width + Height) / 2 - (5 << 1)) / 10;
			var cellControl = new SudokuPaneCell
			{
				CellIndex = i,
				ValueFontSize = valueFontSizeUnified,
				CandidateFontSize = valueFontSizeUnified / 3
			};

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
}
