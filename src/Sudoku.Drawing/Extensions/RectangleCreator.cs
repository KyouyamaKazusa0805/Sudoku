namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="Rectangle"/>.
/// </summary>
/// <seealso cref="Rectangle"/>
internal static class RectangleCreator
{
	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="topLeft">The top-left point.</param>
	/// <param name="bottomRight">The bottom-right point.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Create(Point topLeft, Point bottomRight)
	{
		var (tx, ty) = topLeft;
		var (bx, by) = bottomRight;
		return new(tx, ty, bx - tx, by - ty);
	}

	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="topLeft">The top-left point.</param>
	/// <param name="bottomRight">The bottom-right point.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF Create(PointF topLeft, PointF bottomRight)
	{
		var (tx, ty) = topLeft;
		var (bx, by) = bottomRight;
		return new(tx, ty, bx - tx, by - ty);
	}
}
