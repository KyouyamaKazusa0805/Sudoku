namespace Sudoku.Drawing;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses color value (like type <c>System.Drawing.Color</c>) to distinct with colors.
/// </summary>
/// <param name="alpha">Indicates the color alpha raw values to be assigned.</param>
/// <param name="red">Indicates the color red raw values to be assigned.</param>
/// <param name="green">Indicates the color green raw values to be assigned.</param>
/// <param name="blue">Indicates the color blue raw values to be assigned.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: JsonConstructor]
public sealed partial class ColorColorIdentifier(
	[Property, StringMember] byte alpha,
	[Property, StringMember] byte red,
	[Property, StringMember] byte green,
	[Property, StringMember] byte blue
) : ColorIdentifier
{
	[HashCodeMember]
	private int HashCode => Alpha << 24 | Red << 16 | Green << 8 | Blue;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out byte r, out byte g, out byte b) => (r, g, b) = (Red, Green, Blue);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out byte a, out byte r, out byte g, out byte b) => (a, (r, g, b)) = (Alpha, this);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is ColorColorIdentifier comparer && GetHashCode() == comparer.GetHashCode();
}
