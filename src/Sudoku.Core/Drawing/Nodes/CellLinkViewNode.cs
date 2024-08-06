namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a view node that highlights for a cell link.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="start">Indicates the start point.</param>
/// <param name="end">Indicates the end point.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: JsonConstructor]
public sealed partial class CellLinkViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell start,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell end
) : BasicViewNode(identifier), ILinkViewNode
{
	/// <inheritdoc/>
	object ILinkViewNode.Start => Start;

	/// <inheritdoc/>
	object ILinkViewNode.End => End;

	/// <inheritdoc/>
	Type ILinkViewNode.ElementType => typeof(Cell);


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell start, out Cell end)
		=> (identifier, start, end) = (Identifier, Start, End);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CellLinkViewNode comparer && Start == comparer.Start && End == comparer.End;

	/// <inheritdoc/>
	public override CellLinkViewNode Clone() => new(Identifier, Start, End);
}