namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses palette ID value to distinct with colors.
/// </summary>
/// <param name="value">The palette color ID value to be assigned. The color palette requires implementation of target projects.</param>
[GetHashCode]
[ToString(ToStringBehavior.RecordLike)]
[method: JsonConstructor]
public sealed partial class PaletteIdColorIdentifier([PrimaryCosntructorParameter, HashCodeMember, StringMember] int value) : ColorIdentifier
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out int value) => value = Value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is PaletteIdColorIdentifier comparer && Value == comparer.Value;
}
