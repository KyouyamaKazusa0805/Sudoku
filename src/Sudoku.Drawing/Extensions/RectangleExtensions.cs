namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="Rectangle"/> and <see cref="RectangleF"/>.
/// </summary>
/// <seealso cref="Rectangle"/>
/// <seealso cref="RectangleF"/>
public static class RectangleExtensions
{
	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="leftUp">The left up point.</param>
	/// <param name="rightDown">The right down point.</param>
	/// <returns>The rectangle.</returns>
	public static Rectangle CreateInstance(in Point leftUp, in Point rightDown) =>
		new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);

	/// <summary>
	/// Create an instance with two points.
	/// </summary>
	/// <param name="leftUp">The left up point.</param>
	/// <param name="rightDown">The right down point.</param>
	/// <returns>The rectangle.</returns>
	public static RectangleF CreateInstance(in PointF leftUp, in PointF rightDown) =>
		new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);

	/// <summary>
	/// Zoom in or out the rectangle by the specified offset.
	/// If the offset is positive, the rectangle will be larger; otherwise, smaller.
	/// </summary>
	/// <param name="this">The rectangle.</param>
	/// <param name="offset">The offset to zoom in or out.</param>
	/// <returns>The new rectangle.</returns>
	public static RectangleF Zoom(this in RectangleF @this, float offset)
	{
		var result = @this;
		result.X -= offset;
		result.Y -= offset;
		result.Width += offset * 2;
		result.Height += offset * 2;
		return result;
	}

	/// <summary>
	/// Truncate the specified rectangle.
	/// </summary>
	/// <param name="this">The rectangle.</param>
	/// <returns>The result.</returns>
	public static Rectangle Truncate(this in RectangleF @this) =>
		new((int)@this.X, (int)@this.Y, (int)@this.Width, (int)@this.Height);

#pragma warning disable CS1591
	public static void Deconstruct(this in RectangleF @this, out PointF point, out SizeF size)
	{
		point = new(@this.X, @this.Y);
		size = new(@this.Size);
	}
#pragma warning restore CS1591
}
