namespace Windows.UI;

/// <summary>
/// Provides with extension methods on <see cref="Color"/>.
/// </summary>
/// <seealso cref="Color"/>
internal static partial class ColorExtensions
{
	[GeneratedDeconstruction]
	public static partial void Deconstruct(this Color @this, out byte a, out byte r, out byte g, out byte b);
}
