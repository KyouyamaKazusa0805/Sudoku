namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses color value (like type <c>System.Drawing.Color</c>) to distinct with colors.
/// </summary>
/// <param name="a">Indicates the color alpha raw values to be assigned.</param>
/// <param name="r">Indicates the color red raw values to be assigned.</param>
/// <param name="g">Indicates the color green raw values to be assigned.</param>
/// <param name="b">Indicates the color blue raw values to be assigned.</param>
[method: JsonConstructor]
public sealed partial class ColorColorIdentifier(
	[PrimaryConstructorParameter] byte a,
	[PrimaryConstructorParameter] byte r,
	[PrimaryConstructorParameter] byte g,
	[PrimaryConstructorParameter] byte b
) : ColorIdentifier
{
	/// <summary>
	/// Indicates the raw value.
	/// </summary>
	private int RawValue => A << 24 | R << 16 | G << 8 | B;


	[DeconstructionMethod]
	public partial void Deconstruct(out byte r, out byte g, out byte b);

	[DeconstructionMethod]
	public partial void Deconstruct(out byte a, out byte r, out byte g, out byte b);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is ColorColorIdentifier comparer && RawValue == comparer.RawValue;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.SimpleField, nameof(RawValue))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, "A", "R", "G", "B")]
	public override partial string ToString();
}
