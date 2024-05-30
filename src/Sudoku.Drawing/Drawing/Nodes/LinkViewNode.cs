namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a view node that highlights for a link.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="start">Indicates the start point.</param>
/// <param name="end">Indicates the end point.</param>
/// <param name="inference">Indicates the inference type.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: JsonConstructor]
public sealed partial class LinkViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] LockedTarget start,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] LockedTarget end,
	[PrimaryConstructorParameter, StringMember] Inference inference
) : BasicViewNode(identifier)
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out LockedTarget start, out LockedTarget end, out Inference inference)
		=> (identifier, start, end, inference) = (Identifier, Start, End, Inference);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LinkViewNode comparer && Start == comparer.Start && End == comparer.End;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LinkViewNode Clone() => new(Identifier, Start, End, Inference);
}
