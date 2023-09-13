using Sudoku.Concepts;

namespace Sudoku.Text.Notation;

partial class CellMapNotation
{
	/// <summary>
	/// Represents a kind of output text mode for <see cref="CellMap"/> instances.
	/// </summary>
	/// <seealso cref="CellMap"/>
	public enum Kind
	{
		/// <inheritdoc cref="CellNotation.Kind.RxCy"/>
		RxCy,

		/// <inheritdoc cref="CellNotation.Kind.K9"/>
		K9,

		/// <summary>
		/// Indicates the notation kind is using binary format to output the cells. All values will be treated as a binary digit 0 or 1.
		/// A <see cref="CellMap"/> instance contains 81 bits, so 81 bits will be displayed no matter whether the bit is on.
		/// </summary>
		Binary,

		/// <summary>
		/// Indicates the notation kind is using table format to output the cells. All values will be treated as a digit 0 or 1,
		/// filled with a table.
		/// </summary>
		Table
	}
}
