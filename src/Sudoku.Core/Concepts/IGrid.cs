namespace Sudoku.Concepts;

/// <summary>
/// Represents a base type of a sudoku grid.
/// </summary>
public interface IGrid
{
	/// <summary>
	/// Indicates the number of cells of a sudoku grid.
	/// </summary>
	public const Cell CellsCount = 81;

	/// <summary>
	/// Indicates the number of digits can be appeared inside a cell.
	/// </summary>
	public const Digit CellCandidatesCount = 9;
}
