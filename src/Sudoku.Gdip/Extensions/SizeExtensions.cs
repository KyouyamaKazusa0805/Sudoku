namespace System.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="Size"/>.
/// </summary>
/// <seealso cref="Size"/>
internal static partial class SizeExtensions
{
	[GeneratedDeconstruction]
	public static partial void Deconstruct(this scoped in Size @this, out int width, out int height);
}
