namespace Sudoku.Rendering;

#pragma warning disable CS1591, CS1572

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses palette ID value to distinct with colors.
/// </summary>
/// <param name="value">The palette color ID value to be assigned. The color palette requires implementation of target projects.</param>
public sealed partial class PaletteIdColorIdentifier : ColorIdentifier//([PrimaryConstructorParameter] int value) : ColorIdentifier
{
	[JsonConstructor]
	public PaletteIdColorIdentifier(int value) => Value = value;


	public int Value { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is PaletteIdColorIdentifier comparer && Value == comparer.Value;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.SimpleField, "Value")]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, "Value")]
	public override partial string ToString();
}
