namespace Sudoku.Solving.Dlx;

/// <summary>
/// Represents a column node.
/// </summary>
public sealed class ColumnNode : DancingLinkNode
{
	/// <summary>
	/// Initializes a <see cref="ColumnNode"/> instance via the specified ID value.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	public ColumnNode(Candidate candidate) : base(candidate) => (Column, Size) = (this, 0);


	/// <summary>
	/// Indicates the size of the node.
	/// </summary>
	public int Size { get; set; }


	/// <inheritdoc/>
	public override string ToString() => $"{nameof(ColumnNode)} {{ $Base = {base.ToString()}, {nameof(Size)} = {Size} }}";
}
