#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.Data
{
	/// <summary>
	/// Indicates the grid creating option.
	/// </summary>
#if SOLUTION_WIDE_CODE_ANALYSIS
	[Closed]
#endif
	public enum GridCreatingOption : byte
	{
		/// <summary>
		/// Indicates the option is none.
		/// </summary>
		None,

		/// <summary>
		/// Indicates each value should minus one before creation.
		/// </summary>
		MinusOne
	}
}
