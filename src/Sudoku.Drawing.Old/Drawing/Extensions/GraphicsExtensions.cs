namespace System.Drawing;

/// <summary>
/// Provides extension methods for <see cref="Graphics"/>.
/// </summary>
public static class GraphicsExtensions
{
	/// <summary>
	/// Draw an <see cref="int"/> value onto the current graphics.
	/// </summary>
	/// <param name="this">The graphics instance.</param>
	/// <param name="value">The value to drawing onto.</param>
	/// <param name="font">The font.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="point">The point.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawInt32(this Graphics @this, int value, Font font, Brush brush, in PointF point) =>
		@this.DrawString(value.ToString(), font, brush, point);

	/// <summary>
	/// Draw an <see cref="int"/> value onto the current graphics.
	/// </summary>
	/// <param name="this">The graphics instance.</param>
	/// <param name="value">The value to drawing onto.</param>
	/// <param name="font">The font.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="point">The point.</param>
	/// <param name="stringFormat">The string format instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawInt32(
		this Graphics @this, int value, Font font, Brush brush, in PointF point, StringFormat stringFormat) =>
		@this.DrawString(value.ToString(), font, brush, point, stringFormat);

	/// <summary>
	/// Draw an <see cref="char"/> value onto the current graphics.
	/// </summary>
	/// <param name="this">The graphics instance.</param>
	/// <param name="value">The value to drawing onto.</param>
	/// <param name="font">The font.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="point">The point.</param>
	/// <param name="stringFormat">The string format instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawChar(
		this Graphics @this, char value, Font font, Brush brush, in PointF point, StringFormat stringFormat) =>
		@this.DrawString(value.ToString(), font, brush, point, stringFormat);

	/// <summary>
	/// Draw a cross sign (<c>x</c>).
	/// </summary>
	/// <param name="this">The graphics.</param>
	/// <param name="pen">The pen.</param>
	/// <param name="rectangle">The rectangle.</param>
	/// <remarks>
	/// This method will draw a cross sign and fill with the specified color, so you don't need
	/// to find any fill methods.
	/// </remarks>
	public static void DrawCrossSign(this Graphics @this, Pen pen, in RectangleF rectangle)
	{
		var (x, y, w, h) = rectangle;
		PointF p1 = new(x, y + h), p2 = new(x + w, y), p3 = new(x, y), p4 = new(x + w, y + h);

		@this.DrawLine(pen, p1, p2);
		@this.DrawLine(pen, p3, p4);
	}

	/// <summary>
	/// Draw a capsule.
	/// </summary>
	/// <param name="this">The graphics.</param>
	/// <param name="pen">The pen.</param>
	/// <param name="rectangle">The rectangle.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawCapsule(this Graphics @this, Pen pen, in RectangleF rectangle) =>
		@this.DrawRoundedRectangle(pen, rectangle, 0);

	/// <summary>
	/// Fill a capsule.
	/// </summary>
	/// <param name="this">The graphics.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="rectangle">The rectangle.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FillCapsule(this Graphics @this, Brush brush, in RectangleF rectangle) =>
		@this.FillRoundedRectangle(brush, rectangle, 0);

	/// <summary>
	/// Draw a rounded rectangle.
	/// </summary>
	/// <param name="this">The graphics.</param>
	/// <param name="pen">The pen.</param>
	/// <param name="rectangle">The rectangle to draw.</param>
	/// <param name="circleRadius">The radius of each vertex.</param>
	/// <exception cref="ArgumentException">
	/// Throws when <paramref name="circleRadius"/> is greater than the value in
	/// <paramref name="rectangle"/>.
	/// </exception>
	public static void DrawRoundedRectangle(
		this Graphics @this, Pen pen, in RectangleF rectangle, float circleRadius)
	{
		if (circleRadius > Max(rectangle.Width, rectangle.Height))
		{
			throw new ArgumentOutOfRangeException(nameof(circleRadius));
		}

		RectangleF r1, r2, r3, r4;
		PointF p1, p2, p3, p4, p5, p6, p7, p8;
		p8 = p7 = p6 = p5 = p4 = p3 = p2 = p1 = rectangle.Location;
		p1.X += circleRadius;
		p2.X += rectangle.Width - circleRadius;
		p3.Y += circleRadius;
		p4.X += rectangle.Width;
		p4.Y += circleRadius;
		p5.Y += rectangle.Height - circleRadius;
		p6.X += rectangle.Width;
		p6.Y += rectangle.Height - circleRadius;
		p7.X += circleRadius;
		p7.Y += rectangle.Height;
		p8.X += rectangle.Width - circleRadius;
		p8.Y += rectangle.Height;
		r1 = new(rectangle.X, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r2 = new(
			rectangle.X + rectangle.Width - 2 * circleRadius,
			rectangle.Y, circleRadius * 2, circleRadius * 2);
		r3 = new(
			rectangle.X, rectangle.Y + rectangle.Height - 2 * circleRadius,
			circleRadius * 2, circleRadius * 2);
		r4 = new(
			rectangle.X + rectangle.Width - 2 * circleRadius,
			rectangle.Y + rectangle.Height - 2 * circleRadius,
			circleRadius * 2, circleRadius * 2);

		var path = new GraphicsPath();
		path.AddLine(p1, p2);
		path.AddArc(r2, 270, 90);
		path.AddLine(p4, p6);
		path.AddArc(r4, 0, 90);
		path.AddLine(p7, p8);
		path.AddArc(r3, 90, 90);
		path.AddLine(p5, p3);
		path.AddArc(r1, 180, 90);
		path.CloseFigure();

		@this.DrawPath(pen, path);
	}

	/// <summary>
	/// Fill a rounded rectangle.
	/// </summary>
	/// <param name="this">The graphics.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="rectangle">The rectangle to fill.</param>
	/// <param name="circleRadius">The radius of each vertex.</param>
	/// <exception cref="ArgumentException">
	/// Throws when <paramref name="circleRadius"/> is greater than the value in
	/// <paramref name="rectangle"/>.
	/// </exception>
	public static void FillRoundedRectangle(
		this Graphics @this, Brush brush, in RectangleF rectangle, float circleRadius)
	{
		if (circleRadius >= Max(rectangle.Width, rectangle.Height))
		{
			throw new ArgumentException("Specified argument is greater than the value in rectangle", nameof(circleRadius));
		}

		RectangleF r1, r2, r3, r4;
		PointF p1, p2, p3, p4, p5, p6, p7, p8;
		p8 = p7 = p6 = p5 = p4 = p3 = p2 = p1 = rectangle.Location;
		p1.X += circleRadius;
		p2.X += rectangle.Width - circleRadius;
		p3.Y += circleRadius;
		p4.X += rectangle.Width;
		p4.Y += circleRadius;
		p5.Y += rectangle.Height - circleRadius;
		p6.X += rectangle.Width;
		p6.Y += rectangle.Height - circleRadius;
		p7.X += circleRadius;
		p7.Y += rectangle.Height;
		p8.X += rectangle.Width - circleRadius;
		p8.Y += rectangle.Height;
		r1 = new(rectangle.X, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r2 = new(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r3 = new(rectangle.X, rectangle.Y + rectangle.Height - 2 * circleRadius, circleRadius * 2, circleRadius * 2);
		r4 = new(rectangle.X + rectangle.Width - 2 * circleRadius,
			rectangle.Y + rectangle.Height - 2 * circleRadius,
			circleRadius * 2, circleRadius * 2
		);

		var path = new GraphicsPath();
		path.AddLine(p1, p2);
		path.AddArc(r2, 270, 90);
		path.AddLine(p4, p6);
		path.AddArc(r4, 0, 90);
		path.AddLine(p7, p8);
		path.AddArc(r3, 90, 90);
		path.AddLine(p5, p3);
		path.AddArc(r1, 180, 90);
		path.CloseFigure();

		@this.FillPath(brush, path);
	}
}
