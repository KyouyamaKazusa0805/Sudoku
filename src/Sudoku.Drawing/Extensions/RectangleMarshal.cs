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
	/// <param name="leftUp">The left up point.</param>
	/// <param name="rightDown">The right down point.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle CreateInstance(in Point leftUp, in Point rightDown) =>
		new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);

	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="leftUp">The left up point.</param>
	/// <param name="rightDown">The right down point.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF CreateInstance(in PointF leftUp, in PointF rightDown) =>
		new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);
}
