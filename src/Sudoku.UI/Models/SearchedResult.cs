namespace Sudoku.UI.Models;

/// <summary>
/// Defines a searched result.
/// </summary>
public sealed class SearchedResult
{
	/// <summary>
	/// The value information.
	/// </summary>
	public required string Value { get; set; }

	/// <summary>
	/// The location information.
	/// </summary>
	public required string Location { get; set; }
}
