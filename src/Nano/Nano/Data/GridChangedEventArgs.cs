namespace Nano;

/// <summary>
/// Encapsulates a type that holds a sudoku grid that is used on changing the original grid.
/// </summary>
public sealed class GridChangedEventArgs : RoutedEventArgs
{
	/// <summary>
	/// Initializes a <see cref="GridChangedEventArgs"/> instance via the specified grid puzzle.
	/// </summary>
	/// <param name="grid">The grid.</param>
	public GridChangedEventArgs(in SudokuGrid grid) => NewGrid = grid;


	/// <summary>
	/// Indicates the new grid to change the older one.
	/// </summary>
	public SudokuGrid NewGrid { get; }
}
