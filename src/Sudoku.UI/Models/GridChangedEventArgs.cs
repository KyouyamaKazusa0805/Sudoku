namespace Sudoku.UI.Models;

/// <summary>
/// Provides the data when the event of type <see cref="GridChangedEventHandler"/> is triggered.
/// </summary>
/// <seealso cref="GridChangedEventHandler"/>
public sealed class GridChangedEventArgs : RoutedEventArgs
{
	/// <summary>
	/// Initializes a <see cref="GridChangedEventArgs"/> instance via the specified sudoku grid
	/// of type <see cref="Collections.Grid"/> as the newer one.
	/// </summary>
	/// <param name="grid">The grid that is used for overwritting the older value.</param>
	public GridChangedEventArgs(in Grid grid) => Grid = grid;


	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	public Grid Grid { get; }
}
