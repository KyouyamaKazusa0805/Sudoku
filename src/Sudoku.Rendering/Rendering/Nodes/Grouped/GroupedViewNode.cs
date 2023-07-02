namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a grouped view node.
/// </summary>
/// <param name="identifier">The identifier.</param>
/// <param name="headCell">Indicates the head cell. If the node does not use this property, assign -1.</param>
/// <param name="cells">
/// Indicates the cells used. If the node does not use this property, assign <see cref="ImmutableArray{T}.Empty"/>.
/// </param>
public abstract partial class GroupedViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell headCell,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] ImmutableArray<Cell> cells
) : ViewNode(identifier)
{
	[DeconstructionMethod]
	public partial void Deconstruct([DeconstructionMethodArgument(nameof(HeadCell))] out Cell head, out ImmutableArray<Cell> cells);
}
