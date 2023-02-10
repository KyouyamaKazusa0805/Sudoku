namespace System.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="Size"/>.
/// </summary>
/// <seealso cref="Size"/>
internal static partial class SizeExtensions
{
	[DeconstructionMethod]
	public static partial void Deconstruct(this Size @this, out int width, out int height);
}
