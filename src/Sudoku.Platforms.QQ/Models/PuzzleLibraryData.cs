namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Indicates a puzzle library.
/// </summary>
internal sealed class PuzzleLibraryData
{
	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the name of the group ID.
	/// </summary>
	[JsonPropertyOrder(1)]
	public required string GroupId { get; set; }

	/// <summary>
	/// Indicates the description of the puzzle library.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the puzzles in the library.
	/// </summary>
	public required string PuzzleFilePath { get; set; }

	/// <summary>
	/// Indicates the tags of the library. For example, "Hard".
	/// </summary>
	public string[]? Tags { get; set; }

	/// <summary>
	/// Indicates how many puzzles the library has been finished.
	/// </summary>
	public int FinishedPuzzlesCount { get; set; }
}
