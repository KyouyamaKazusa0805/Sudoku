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
	/// Indicates the puzzles in the library.
	/// </summary>
	[JsonPropertyName("puzzlePath")]
	public required string PuzzleFilePath { get; set; }
}
