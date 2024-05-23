namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents a type of notation to describe a coordinate in sudoku.
/// </summary>
public enum CoordinateType
{
	/// <summary>
	/// Idnicates the notation is based on literally notation.
	/// </summary>
	Literal,

	/// <summary>
	/// Indicates the notation is based on <b>RxCy</b> notation.
	/// </summary>
	RxCy,

	/// <summary>
	/// Indicates the notation is based on <b>K9</b> notation.
	/// </summary>
	K9,

	/// <summary>
	/// Indicates the notation is based on <b>Excel</b> notation.
	/// </summary>
	Excel
}
