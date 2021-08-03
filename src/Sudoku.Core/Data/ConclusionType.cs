#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.Data
{
	/// <summary>
	/// Provides a conclusion type.
	/// </summary>
#if SOLUTION_WIDE_CODE_ANALYSIS
	[Closed]
#endif
	public enum ConclusionType : byte
	{
		/// <summary>
		/// Indicates the conclusion is a value filling into a cell.
		/// </summary>
		Assignment,

		/// <summary>
		/// Indicates the conclusion is a candidate being remove from a cell.
		/// </summary>
		Elimination,
	}
}
