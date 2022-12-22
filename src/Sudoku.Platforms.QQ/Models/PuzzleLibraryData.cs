namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Indicates a puzzle library.
/// </summary>
public sealed class PuzzleLibraryData
{
	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	[JsonPropertyName("name")]
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the description of the puzzle library.
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the puzzles in the library.
	/// </summary>
	[JsonPropertyName("puzzlePath")]
	public required string PuzzleFilePath { get; set; }

	/// <summary>
	/// Indicates how many puzzles the library has been finished.
	/// </summary>
	[JsonPropertyName("finishedCount")]
	public int FinishedPuzzlesCount { get; set; }
}
