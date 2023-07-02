namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines an average bar view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="isHorizontal">Indicates whether the view node is for horizontal one.</param>
[GetHashCode]
[ToString]
public sealed partial class AverageBarViewNode(
	ColorIdentifier identifier,
	Cell cell,
	[PrimaryConstructorParameter, HashCodeMember] bool isHorizontal
) : SingleCellMarkViewNode(identifier, cell, Direction.None)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is AverageBarViewNode comparer && Cell == comparer.Cell && IsHorizontal == comparer.IsHorizontal;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override AverageBarViewNode Clone() => new(Identifier, Cell, IsHorizontal);
}
