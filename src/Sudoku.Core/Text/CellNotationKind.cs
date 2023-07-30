namespace Sudoku.Text;

/// <summary>
/// Represents a kind of the cell notation.
/// </summary>
public enum CellNotationKind
{
	/// <summary>
	/// Indicates the cell notation is <see href="http://sudopedia.enjoysudoku.com/Rncn.html">RxCy Notation</see>.
	/// </summary>
	/// <remarks>
	/// The <b>RxCy notation</b> is a notation to describe a set of cells that uses letter
	/// <c>R</c> (or its lower case <c>r</c>) to describe a row label, and uses the other letter
	/// <c>C</c> (or its lower case <c>c</c>) to describe a column label. For example,
	/// <c>R4C2</c> means the cell at row 4 and column 2.
	/// </remarks>
	RxCy,

	/// <summary>
	/// Indicates the cell notation is <see href="http://sudopedia.enjoysudoku.com/K9.html">K9 Notation</see>.
	/// </summary>
	/// <remarks>
	/// The <b>K9 notation</b> is a notation to describe a set of cells that uses letters
	/// A, B, C, D, E, F, G, H and K to describe the row, and uses digits 1 to 9 to describe the column.
	/// For example, <c>C8</c> means the cell at row 3 and column 8.
	/// The letter I and J aren't used in this notation because they are confusing with digit 1.
	/// However, they can also be used in Chinese notations, for example, <c>K8</c> in traditional notation
	/// is same as <c>I8</c> in Chinese K9 notation rule.
	/// </remarks>
	K9
}
