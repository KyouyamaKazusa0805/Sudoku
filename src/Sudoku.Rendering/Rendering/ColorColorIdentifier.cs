namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses color value (like type <c>System.Drawing.Color</c>) to distinct with colors.
/// </summary>
/// <param name="a">The color alpha raw values to be assigned.</param>
/// <param name="r">The color red raw values to be assigned.</param>
/// <param name="g">The color green raw values to be assigned.</param>
/// <param name="b">The color blue raw values to be assigned.</param>
public sealed partial class ColorColorIdentifier(byte a, byte r, byte g, byte b) : ColorIdentifier
{
	/// <summary>
	/// Indicates the alpha value of the color.
	/// </summary>
	public byte A { get; } = a;

	/// <summary>
	/// Indicates the red value of the color.
	/// </summary>
	public byte R { get; } = r;

	/// <summary>
	/// Indicates the green value of the color.
	/// </summary>
	public byte G { get; } = g;

	/// <summary>
	/// Indicates the blue value of the color.
	/// </summary>
	public byte B { get; } = b;

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

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(A), nameof(R), nameof(G), nameof(B))]
	public override partial string ToString();
}
