#if DOUBLE_LAYERED_ASSUMPTION

using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Data
{
	/// <summary>
	/// Indicates a case of the advanced forcing chains.
	/// </summary>
	[Closed]
	public enum Cause : byte
	{
		/// <summary>
		/// Indicates the node doesn't contain any causes.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the hidden single in row.
		/// </summary>
		RowHiddenSingle,

		/// <summary>
		/// Indicates the hidden single in column.
		/// </summary>
		ColumnHiddenSingle,

		/// <summary>
		/// Indicates the hidden single in block.
		/// </summary>
		BlockHiddenSingle,

		/// <summary>
		/// Indicates the naked single.
		/// </summary>
		NakedSingle,

		/// <summary>
		/// Indicates the advanced cause.
		/// </summary>
		Advanced
	}
}

#endif