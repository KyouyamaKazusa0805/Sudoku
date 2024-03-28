namespace Sudoku.Generating;

/// <summary>
/// Represents a full puzzle.
/// </summary>
[GetHashCode]
[ToString(ToStringBehavior.RecordLike)]
public sealed partial class FullPuzzle : PuzzleBase
{
	/// <summary>
	/// Indicates the solution to the puzzle.
	/// </summary>
	public Grid Solution => Puzzle.SolutionGrid;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] PuzzleBase? other)
		=> other is FullPuzzle comparer && (Result, Puzzle) == (comparer.Result, comparer.Puzzle);
}
