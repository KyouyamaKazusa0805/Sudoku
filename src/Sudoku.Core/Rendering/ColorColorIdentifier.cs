namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses color value (like type <c>System.Drawing.Color</c>) to distinct with colors.
/// </summary>
/// <param name="alpha">Indicates the color alpha raw values to be assigned.</param>
/// <param name="red">Indicates the color red raw values to be assigned.</param>
/// <param name="green">Indicates the color green raw values to be assigned.</param>
/// <param name="blue">Indicates the color blue raw values to be assigned.</param>
[method: JsonConstructor]
public sealed class ColorColorIdentifier(byte alpha, byte red, byte green, byte blue) : ColorIdentifier
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out byte r, out byte g, out byte b) => (r, g, b) = (red, green, blue);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out byte a, out byte r, out byte g, out byte b) => (a, (r, g, b)) = (alpha, this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is ColorColorIdentifier comparer && GetHashCode() == comparer.GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => alpha << 24 | red << 16 | green << 8 | blue;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"A = {alpha}, @ref = {red}, G = {green}, B = {blue}";
}
