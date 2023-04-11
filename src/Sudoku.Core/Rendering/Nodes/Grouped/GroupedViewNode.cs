namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a grouped view node.
/// </summary>
/// <param name="identifier">The identifier.</param>
/// <param name="headCell">The head cell.</param>
/// <param name="cells">The cells.</param>
public abstract partial class GroupedViewNode(Identifier identifier, int headCell, ImmutableArray<int> cells) : ViewNode(identifier)
{
	/// <summary>
	/// Indicates the head cell. If the node does not use this property, assign -1.
	/// </summary>
	public int HeadCell { get; } = headCell;

	/// <summary>
	/// Indicates the cells used. If the node does not use this property, assign <see cref="ImmutableArray{T}.Empty"/>.
	/// </summary>
	/// <seealso cref="ImmutableArray{T}.Empty"/>
	public ImmutableArray<int> Cells { get; } = cells;


	[DeconstructionMethod]
	public partial void Deconstruct([DeconstructionMethodArgument(nameof(HeadCell))] out int head, out ImmutableArray<int> cells);
}
