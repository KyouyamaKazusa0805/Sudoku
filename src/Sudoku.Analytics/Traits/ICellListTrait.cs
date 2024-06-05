namespace Sudoku.Traits;

/// <summary>
/// Represents a trait that describes the cell list.
/// </summary>
public interface ICellListTrait : ITrait
{
	/// <summary>
	/// Indicates the number of cells.
	/// </summary>
	public abstract int CellSize { get; }
}
