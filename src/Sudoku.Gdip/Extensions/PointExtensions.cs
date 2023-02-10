namespace System.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="Point"/>.
/// </summary>
/// <seealso cref="Point"/>
internal static partial class PointExtensions
{
	[DeconstructionMethod]
	public static partial void Deconstruct(this Point @this, out int x, out int y);
}
