namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Represents a grid parsing type.
	/// </summary>
	public enum GridParsingType : byte
	{
		/// <summary>
		/// Indicates the susser format, which means all grid values
		/// will be displayed in one line with empty cell character
		/// <c>'0'</c> or <c>'.'</c>.
		/// </summary>
		Susser,
		/// <summary>
		/// Indicates the pencil marked grid (PM grid), which means all
		/// grid candidates will be displayed using a table.
		/// </summary>
		PencilMarked,
		/// <summary>
		/// Indicates the pencil marked grid (PM grid), which means all
		/// grid candidates will be displayed using a table. In addition,
		/// all single digit will be treated as a given digit.
		/// </summary>
		PencilMarkedTreatSingleAsGiven,
		/// <summary>
		/// Indicates the table format, which means all grid values
		/// will be displayed using a table with empty cell character
		/// <c>'0'</c> or <c>'.'</c>.
		/// </summary>
		Table,
	}
}
