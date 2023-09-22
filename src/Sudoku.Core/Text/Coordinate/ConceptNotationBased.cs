namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents a kind of notation to describe a concept in sudoku.
/// </summary>
public enum ConceptNotationBased
{
	/// <summary>
	/// Idnicates the notation is based on literally notation.
	/// </summary>
	LiteralBased,

	/// <summary>
	/// Indicates the notation is based on <b>RxCy</b> notation.
	/// </summary>
	RxCyBased,

	/// <summary>
	/// Indicates the notation is based on <b>K9</b> notation.
	/// </summary>
	K9Based,

	/// <summary>
	/// Indicates the notation is based on <b>Excel</b> notation.
	/// </summary>
	ExcelBased
}
