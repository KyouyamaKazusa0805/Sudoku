namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// The base overrides for double exocet steps.
/// </summary>
internal interface IDoubleExocet
{
	/// <summary>
	/// A list of cells as the base cells.
	/// </summary>
	public abstract CellMap BaseCells { get; }

	/// <summary>
	/// A list of cells as the target cells.
	/// </summary>
	public abstract CellMap TargetCells { get; }

	/// <summary>
	/// A list of cells as the other pair of base cells.
	/// </summary>
	public abstract CellMap BaseCellsTheOther { get; }

	/// <summary>
	/// A list of cells as the other pair of target cells.
	/// </summary>
	public abstract CellMap TargetCellsTheOther { get; }
}
