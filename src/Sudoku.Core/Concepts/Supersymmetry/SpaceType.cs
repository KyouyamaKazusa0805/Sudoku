namespace Sudoku.Concepts.Supersymmetry;

/// <summary>
/// Represents a type of space.
/// </summary>
public enum SpaceType : byte
{
	/// <summary>
	/// Indicates row-column space (RC space). The notation will be <c>XnY</c>, meaning cell at row X and column Y
	/// (equivalent to <c>rXcY</c>).
	/// </summary>
	RowColumn,

	/// <summary>
	/// Indicates row-number space (RN space). The notation will be <c>XrY</c>, meaning digit X in row Y.
	/// </summary>
	RowNumber,

	/// <summary>
	/// Indicates column-number space (CN space). The notation will be <c>XcY</c>, meaning digit X in column Y.
	/// </summary>
	ColumnNumber,

	/// <summary>
	/// Indicates block-number space (BN space). The notation will be <c>XbY</c>, meaning digit X in block Y.
	/// </summary>
	BlockNumber
}
