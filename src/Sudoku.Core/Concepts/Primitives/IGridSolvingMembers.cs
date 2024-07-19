namespace Sudoku.Concepts.Primitives;

/// <summary>
/// Represents a grid that contains solving members.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface IGridSolvingMembers<TSelf> where TSelf : IGridSolvingMembers<TSelf>
{
	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>, the grid is solved;
	/// otherwise, <see langword="false"/>.
	/// </summary>
	public abstract bool IsSolved { get; }
}
