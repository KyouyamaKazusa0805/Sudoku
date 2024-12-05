namespace Sudoku.Concepts.Specialized;

/// <summary>
/// Represents constants of <see cref="AdjacentCellDirection"/>.
/// </summary>
/// <seealso cref="AdjacentCellDirection"/>
public static class AdjacentCellDirections
{
	/// <summary>
	/// Indicates all directions.
	/// </summary>
	public const AdjacentCellDirection All = AdjacentCellDirection.Up | AdjacentCellDirection.Down | AdjacentCellDirection.Left | AdjacentCellDirection.Right;
}
