using Sudoku.Concepts;

namespace Sudoku.Analytics.InternalAbstraction;

/// <summary>
/// The base overrides for double exocet steps.
/// </summary>
internal interface IDoubleExocetStepBaseOverrides
{
	public abstract CellMap BaseCells { get; }

	public abstract CellMap TargetCells { get; }

	public abstract CellMap BaseCellsTheOther { get; }

	public abstract CellMap TargetCellsTheOther { get; }
}
