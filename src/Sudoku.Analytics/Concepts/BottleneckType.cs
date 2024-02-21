namespace Sudoku.Concepts;

/// <summary>
/// Represents a kind of bottleneck.
/// </summary>
[Flags]
public enum BottleneckType
{
	/// <summary>
	/// Indicates the placeholder of this type.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the bottleneck is for direct views.
	/// </summary>
	DirectBottleneck = 1,

	/// <summary>
	/// Indicates the bottleneck is for a whole puzzle.
	/// </summary>
	PuzzleBottleneck = 1 << 1
}
