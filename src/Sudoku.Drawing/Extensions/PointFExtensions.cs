namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="PointF"/>.
/// </summary>
/// <seealso cref="PointF"/>
internal static class PointFExtensions
{
	/// <summary>
	/// To truncate the point.
	/// </summary>
	/// <param name="this">The point to truncate.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Truncate(this PointF @this) => new((int)@this.X, (int)@this.Y);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this PointF @this, out float x, out float y) => (x, y) = (@this.X, @this.Y);
}
