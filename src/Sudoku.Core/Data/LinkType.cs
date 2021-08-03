#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.Data
{
	/// <summary>
	/// Indicates a link type.
	/// </summary>
#if SOLUTION_WIDE_CODE_ANALYSIS
	[Closed]
#endif
	public enum LinkType : byte
	{
		/// <summary>
		/// Indicates the default link (<c><![CDATA[off -> off]]></c> or <c><![CDATA[on -> on]]></c>).
		/// </summary>
		Default,

		/// <summary>
		/// Indicates the weak link (<c><![CDATA[on -> off]]></c>).
		/// </summary>
		Weak,

		/// <summary>
		/// Indicates the strong link (<c><![CDATA[off -> on]]></c>).
		/// </summary>
		Strong,

		/// <summary>
		/// Indicates the link is used for rendering as a normal line (without start and end node).
		/// </summary>
		Line,
	}
}
