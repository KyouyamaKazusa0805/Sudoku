namespace Windows.UI;

/// <summary>
/// Provides with extension methods on <see cref="Color"/>.
/// </summary>
/// <seealso cref="Color"/>
public static class ColorExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Color @this, out byte r, out byte g, out byte b) => (r, g, b) = (@this.R, @this.G, @this.B);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Color @this, out byte a, out byte r, out byte g, out byte b)
		=> (a, r, g, b) = (@this.A, @this.R, @this.G, @this.B);

	/// <summary>
	/// Converts the specified color into equivalent <see cref="SKColor"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="Color"/> instance.</param>
	/// <returns>Final <see cref="SKColor"/> instance.</returns>
	public static SKColor AsSKColor(this Color @this)
	{
		var (a, r, g, b) = @this;
		return new(r, g, b, a);
	}

	/// <summary>
	/// Gets an equivalent <see cref="ColorIdentifier"/> instance via the current color.
	/// </summary>
	/// <param name="color">The color.</param>
	/// <returns>An <see cref="ColorIdentifier"/> instance.</returns>
	public static ColorIdentifier GetIdentifier(this Color color)
	{
		var (a, r, g, b) = color;
		return new ColorColorIdentifier(a, r, g, b);
	}
}
