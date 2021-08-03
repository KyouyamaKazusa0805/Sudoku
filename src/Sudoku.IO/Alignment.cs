#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.IO
{
	/// <summary>
	/// Indicates an alignment type.
	/// </summary>
#if SOLUTION_WIDE_CODE_ANALYSIS
	[Closed]
#endif
	public enum Alignment : byte
	{
		/// <summary>
		/// Indicates the left alignment.
		/// </summary>
		Left,

		/// <summary>
		/// Indicates the middle alignment.
		/// </summary>
		Middle,

		/// <summary>
		/// Indicates the right alignment.
		/// </summary>
		Right
	}
}
