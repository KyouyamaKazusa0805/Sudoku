namespace SudokuStudio.Models;

/// <summary>
/// Indicates the puzzle-generating history.
/// </summary>
public sealed class PuzzleGenertingHistory
{
	/// <summary>
	/// Indicates the puzzle strings.
	/// </summary>
	public List<GridSerializationData> Puzzles { get; } = new();
}
