namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a link.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="start">Indicates the start point.</param>
/// <param name="end">Indicates the end point.</param>
/// <param name="inference">Indicates the inference type.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class LinkViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] LockedTarget start,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] LockedTarget end,
	[PrimaryConstructorParameter, StringMember] Inference inference
) : BasicViewNode(identifier)
{
	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out LockedTarget start, out LockedTarget end, out Inference inference);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LinkViewNode comparer && Start == comparer.Start && End == comparer.End;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LinkViewNode Clone() => new(Identifier, Start, End, Inference);
}
