namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="Rectangle"/>.
/// </summary>
/// <seealso cref="Rectangle"/>
internal static class RectangleMarshal
{
	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="topLeft">The top-left point.</param>
	/// <param name="bottomRight">The bottom-right point.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle CreateInstance(Point topLeft, Point bottomRight)
		=> new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="topLeft">The top-left point.</param>
	/// <param name="buttomRight">The bottom-right point.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF CreateInstance(PointF topLeft, PointF buttomRight)
		=> new(topLeft.X, topLeft.Y, buttomRight.X - topLeft.X, buttomRight.Y - topLeft.Y);
}
