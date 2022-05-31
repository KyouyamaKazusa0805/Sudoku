namespace Sudoku.UI.Models;

/// <summary>
/// Defines a searched result.
/// </summary>
public sealed class SearchedResult
{
	/// <summary>
	/// The value information.
	/// </summary>
	public string Value { get; set; } = string.Empty;

	/// <summary>
	/// The location information.
	/// </summary>
	public string Location { get; set; } = string.Empty;
}
