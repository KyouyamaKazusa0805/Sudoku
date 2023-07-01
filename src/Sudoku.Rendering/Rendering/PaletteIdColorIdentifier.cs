namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses palette ID value to distinct with colors.
/// </summary>
/// <param name="value">The palette color ID value to be assigned. The color palette requires implementation of target projects.</param>
[GetHashCode]
[method: JsonConstructor]
public sealed partial class PaletteIdColorIdentifier([PrimaryConstructorParameter, HashCodeMember] int value) : ColorIdentifier
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is PaletteIdColorIdentifier comparer && Value == comparer.Value;

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, "Value")]
	public override partial string ToString();
}
