namespace Sudoku.IO;

/// <summary>
/// Indicates the analysis result output type.
/// </summary>
[Closed]
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
