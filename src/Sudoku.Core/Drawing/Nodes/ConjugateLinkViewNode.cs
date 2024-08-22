namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a view node that highlights for a conjugate pair.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="start">Indicates the start point.</param>
/// <param name="end">Indicates the end point.</param>
/// <param name="digit">Indicates the digit used.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: JsonConstructor]
public sealed partial class ConjugateLinkViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell start,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell end,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Digit digit
) : ViewNode(identifier), ILinkViewNode
{
	/// <summary>
	/// Indicates the target conjugate pair.
	/// </summary>
	public Conjugate ConjugatePair => new(Start, End, Digit);

	/// <inheritdoc/>
	object ILinkViewNode.Start => Start * 9 + Digit;

	/// <inheritdoc/>
	object ILinkViewNode.End => End * 9 + Digit;

	/// <inheritdoc/>
	Type ILinkViewNode.ElementType => typeof(Conjugate);


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell start, out Cell end, out Digit digit)
		=> (identifier, start, end, digit) = (Identifier, Start, End, Digit);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ConjugateLinkViewNode comparer && Start == comparer.Start && End == comparer.End && Digit == comparer.Digit;

	/// <inheritdoc/>
	public override ConjugateLinkViewNode Clone() => new(Identifier, Start, End, Digit);
}
