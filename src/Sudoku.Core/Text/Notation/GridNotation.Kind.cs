namespace Sudoku.Text.Notation;

partial class GridNotation
{
	/// <summary>
	/// Represents a kind of output text mode for <see cref="Grid"/> instances.
	/// </summary>
	/// <seealso cref="Grid"/>
	public enum Kind
	{
		/// <summary>
		/// Indicates the Susser format.
		/// </summary>
		Susser,

		/// <summary>
		/// Indicates the Susser format, treating all values as givens.
		/// </summary>
		SusserTreatingValuesAsGivens,

		/// <summary>
		/// Indicates elimination notation that follows Susser format.
		/// </summary>
		SusserElimination,

		/// <summary>
		/// Indicates pencilmark format.
		/// </summary>
		Pencilmark,

		/// <summary>
		/// Indicates Hodoku library format.
		/// </summary>
		HodokuLibrary,

		/// <summary>
		/// Indicates multiple line format.
		/// </summary>
		MultipleLine,

		/// <summary>
		/// Indicates sukaku format.
		/// </summary>
		Sukaku,

		/// <summary>
		/// Indicates Excel format.
		/// </summary>
		Excel,

		/// <summary>
		/// Indicates Open-Sudoku format.
		/// </summary>
		OpenSudoku
	}
}
