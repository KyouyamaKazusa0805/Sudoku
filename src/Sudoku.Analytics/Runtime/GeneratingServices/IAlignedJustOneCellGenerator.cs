namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a type that can generate puzzles that can only be solved for one cell, with phased grid and its solving step.
/// </summary>
public interface IAlignedJustOneCellGenerator : IJustOneCellGenerator
{
	/// <summary>
	/// Indicates the alignment value that the target generated puzzle will align its conclusion cell.
	/// </summary>
	public abstract ConclusionCellAlignment Alignment { get; set; }
}
