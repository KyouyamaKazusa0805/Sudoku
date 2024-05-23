namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents a type of sudoku puzzle.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum SudokuType
{
	/// <summary>
	/// The placeholder of the enumeration field.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the sudoku puzzle is a standard.
	/// </summary>
	Standard = 1 << 0,

	/// <summary>
	/// Indicates the sudoku puzzle is a sukaku (pencilmark sudoku).
	/// </summary>
	Sukaku = 1 << 1,

	/// <summary>
	/// Indicates the sudoku puzzle is a just-one-cell sudoku.
	/// </summary>
	JustOneCell = 1 << 2
}
