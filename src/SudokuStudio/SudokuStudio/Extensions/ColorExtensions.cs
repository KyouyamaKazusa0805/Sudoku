namespace Windows.UI;

/// <summary>
/// Provides with extension methods on <see cref="Color"/>.
/// </summary>
/// <seealso cref="Color"/>
public static partial class ColorExtensions
{
	[GeneratedDeconstruction]
	public static partial void Deconstruct(this Color @this, out byte r, out byte g, out byte b);

	[GeneratedDeconstruction]
	public static partial void Deconstruct(this Color @this, out byte a, out byte r, out byte g, out byte b);

	/// <summary>
	/// Converts the specified color into equivalent <see cref="SKColor"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="Color"/> instance.</param>
	/// <returns>Final <see cref="SKColor"/> instance.</returns>
	public static SKColor AsSKColor(this Color @this)
	{
		_ = @this is var (a, r, g, b);
		return new(r, g, b, a);
	}
}
