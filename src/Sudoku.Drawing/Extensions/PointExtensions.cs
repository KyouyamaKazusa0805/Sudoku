namespace System.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="Point"/>.
/// </summary>
/// <seealso cref="Point"/>
internal static class PointExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Point @this, out int x, out int y) => (x, y) = (@this.X, @this.Y);
}
