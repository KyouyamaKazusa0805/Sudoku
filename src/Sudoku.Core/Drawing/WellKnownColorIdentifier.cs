namespace Sudoku.Drawing;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses well-known kinds to distinct with colors.
/// </summary>
/// <param name="kind">The well-known identifier kind to be assigned.</param>
[TypeImpl(TypeImplFlags.Object_GetHashCode | TypeImplFlags.Object_ToString, ToStringBehavior = ToStringBehavior.RecordLike)]
[method: JsonConstructor]
public sealed partial class WellKnownColorIdentifier([Property, HashCodeMember, StringMember] WellKnownColorIdentifierKind kind) :
	ColorIdentifier
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out WellKnownColorIdentifierKind kind) => kind = Kind;

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is WellKnownColorIdentifier comparer && Kind == comparer.Kind;
}
