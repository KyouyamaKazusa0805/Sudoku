namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	private Grid _puzzle;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane() => InitializeComponent();


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
		}
	}


	/// <summary>
	/// Initializes for cell data.
	/// </summary>
	private void InitializeCellData()
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = new SudokuPaneCell { CellIndex = i };

			GridLayout.SetRow(cellControl, i / 9 + 2);
			GridLayout.SetColumn(cellControl, i % 9 + 2);

			MainGrid.Children.Add(cellControl);
		}
	}

	/// <summary>
	/// To initialize <see cref="GridCellData"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void UpdateCellData(Grid grid)
	{
		if (DataContext is not SudokuPaneBindingContext { Cells: { } cells })
		{
			return;
		}

		cells.ForEach((cell, i) => { cell.CandidatesMask = grid.GetCandidates(i); cell.CellStatus = grid.GetStatus(i); });
	}


	private void UserControl_Loaded(object sender, RoutedEventArgs e) => InitializeCellData();

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

	private void UserControl_PointerMoved(object sender, PointerRoutedEventArgs e)
		=> Context.SelectedCell = Context.Cells.FirstOrDefault(static data => data.IsMouseHovered)?.CellIndex ?? -1;
}
