#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.IO
{
	/// <summary>
	/// Indicates the analysis result output type.
	/// </summary>
#if SOLUTION_WIDE_CODE_ANALYSIS
	[Closed]
#endif
	public enum AnalysisResultOutputType : byte
	{
		/// <summary>
		/// Indicates the word document format (<c>*.docx</c>).
		/// </summary>
		WordDocument,

		/// <summary>
		/// Indicates the text file format (<c>*.txt</c>).
		/// </summary>
		Text,
	}
}
