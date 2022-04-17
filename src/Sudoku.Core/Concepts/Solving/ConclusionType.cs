namespace Sudoku.Concepts.Solving;

/// <summary>
/// Provides a conclusion type.
/// </summary>
public enum ConclusionType : byte
{
	/// <summary>
	/// Indicates the conclusion is a value filling into a cell.
	/// </summary>
	[EnumFieldName(" = ")]
	Assignment,

	/// <summary>
	/// Indicates the conclusion is a candidate being remove from a cell.
	/// </summary>
	[EnumFieldName(" <> ")]
	Elimination
}
