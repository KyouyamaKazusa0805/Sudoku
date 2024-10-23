namespace Sudoku.Concepts.Supersymmetry;

/// <summary>
/// Represents a type of space.
/// </summary>
public enum SpaceType : byte
{
	/// <summary>
	/// Indicates row-column space (RC space).
	/// </summary>
	RowColumn,

	/// <summary>
	/// Indicates row-number space (RN space).
	/// </summary>
	RowNumber,

	/// <summary>
	/// Indicates column-number space (CN space).
	/// </summary>
	ColumnNumber,

	/// <summary>
	/// Indicates block-number space (BN space).
	/// </summary>
	BlockNumber
}
