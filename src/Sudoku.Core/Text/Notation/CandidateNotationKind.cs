namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a kind of the candidate notation.
/// </summary>
public enum CandidateNotationKind
{
	/// <summary>
	/// Indicates the candidate notation is <see href="http://sudopedia.enjoysudoku.com/Rncn.html">RxCy Notation</see>.
	/// </summary>
	/// <remarks>
	/// The <b>RxCy notation</b> is a notation to describe a set of cells that uses letter
	/// <c>R</c> (or its lower case <c>r</c>) to describe a row label, and uses the other letter
	/// <c>C</c> (or its lower case <c>c</c>) to describe a column label, and after the cell notation, with a brace surrounding.
	/// For example, <c>R4C2(8)</c> means digit 8 in the cell at row 4 and column 2.
	/// </remarks>
	RxCy,

	/// <summary>
	/// Indicates the candidate notation is <see href="http://sudopedia.enjoysudoku.com/K9.html">K9 Notation</see>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>K9 notation</b> is a notation to describe a set of cells that uses letters
	/// A, B, C, D, E, F, G, H and K to describe the row, and uses digits 1 to 9 to describe the column.
	/// A digit will be used after the cell notation, with a dot before the digit.
	/// For example, <c>C8.1</c> means digit 1 in the cell at row 3 and column 8.
	/// </para>
	/// <para>
	/// The letter I and J aren't used in this notation because they are confusing with digit 1.
	/// However, they can also be used in Chinese notations. For example, <c>K8.1</c> in traditional notation
	/// is same as <c>I8.1</c> in Chinese K9 notation rule.
	/// </para>
	/// </remarks>
	K9,

	/// <summary>
	/// Indicates the candidate notation is Hodoku digit triplet notation.
	/// </summary>
	/// <remarks>
	/// The <b>Triplet notation</b> is a notation to describe a single candidate using 3 digits without any spaces.
	/// The ordering of three digits are <i>digit</i>, <i>row</i> and <i>column</i>.
	/// For example, <c>123</c> means digit 1 in the cell at the row 2 and column 3.
	/// </remarks>
	HodokuTriplet
}
