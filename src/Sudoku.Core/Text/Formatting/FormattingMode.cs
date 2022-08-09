namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents for a formatting mode (simple, normal or full). The modes will correspond to the methods
/// <c>ToSimpleString</c>, <c>ToString</c> and <c>ToFullString</c>.
/// </summary>
public enum FormattingMode
{
	/// <summary>
	/// Indicates the formatting mode is simple, which means the result string will be more simple
	/// that options <see cref="Normal"/> or <see cref="Full"/> behaves.
	/// </summary>
	Simple,

	/// <summary>
	/// Indicates the formatting mode is normal, which means all important data will be displayed.
	/// </summary>
	Normal,

	/// <summary>
	/// Indicates the formatting mode is full, which means all possible data will be displayed.
	/// </summary>
	Full
}
