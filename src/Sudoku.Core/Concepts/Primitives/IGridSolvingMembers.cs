namespace Sudoku.Concepts.Primitives;

/// <summary>
/// Represents a grid that contains solving members.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface IGridSolvingMembers<TSelf> : IGridConstants<TSelf> where TSelf : unmanaged, IGridSolvingMembers<TSelf>
{
	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>, the grid is solved;
	/// otherwise, <see langword="false"/>.
	/// </summary>
	public abstract bool IsSolved { get; }
}
