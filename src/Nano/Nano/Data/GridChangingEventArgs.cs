namespace Nano;

/// <summary>
/// Encapsulates a type that holds a sudoku grid that is used on changing the original grid.
/// </summary>
public sealed class GridChangingEventArgs : RoutedEventArgs
{
	/// <summary>
	/// Initializes a <see cref="GridChangedEventArgs"/> instance via the specified grid puzzle.
	/// </summary>
	/// <param name="grid">The grid.</param>
	public GridChangingEventArgs(in SudokuGrid grid) => OldGrid = grid;


	/// <summary>
	/// Indicates the original grid to be replaced later.
	/// </summary>
	public SudokuGrid OldGrid { get; }
}