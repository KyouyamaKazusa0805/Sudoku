namespace System.Drawing;

/// <summary>
/// Provides extension methods for <see cref="Graphics"/>.
/// </summary>
internal static class GraphicsExtensions
{
	/// <summary>
	/// Draws an image onto the target <see cref="Graphics"/> instance, with specified customized data.
	/// </summary>
	/// <param name="this">The <see cref="Graphics"/> instance.</param>
	/// <param name="bitmap">The image to be used for drawing onto the target <see cref="Graphics"/> instance.</param>
	/// <param name="destX">The X coordinate of the destination point.</param>
	/// <param name="destY">The Y coordinate of the destination point.</param>
	/// <param name="destWidth">The width of the destination point.</param>
	/// <param name="destHeight">The height of the destination point.</param>
	/// <param name="srcX">The X coordinate of the source point.</param>
	/// <param name="srcY">The Y coordinate of the source point.</param>
	/// <param name="srcWidth">The width of the source point.</param>
	/// <param name="srcHeight">The height of the source point.</param>
	/// <param name="srcUnit">
	/// The source graphics unit <see cref="GraphicsUnit"/> instance indicating which kind of unit those coordinate arguments
	/// (e.g. <paramref name="destX"/>, <paramref name="destY"/>, <paramref name="srcX"/>, <paramref name="srcY"/>, etc.) uses.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawImage(
		this Graphics @this,
		Bitmap bitmap,
		int destX, int destY, int destWidth, int destHeight,
		int srcX, int srcY, int srcWidth, int srcHeight,
		GraphicsUnit srcUnit
	) => @this.DrawImage(bitmap, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(srcX, srcY, srcWidth, srcHeight), srcUnit);

	/// <summary>
	/// Draw the string representation an instance onto the current <see cref="Graphics"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of the value to draw.</typeparam>
	/// <param name="this">The graphics instance.</param>
	/// <param name="value">The value to drawing onto.</param>
	/// <param name="font">The font.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="point">The point.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawValue<T>(this Graphics @this, T value, Font font, Brush brush, scoped in PointF point) where T : notnull
		=> @this.DrawString(value.ToString(), font, brush, point);

	/// <summary>
	/// Draw the string representation of an instance onto the current <see cref="Graphics"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of the value to draw.</typeparam>
	/// <param name="this">The graphics instance.</param>
	/// <param name="value">The value to drawing onto.</param>
	/// <param name="font">The font.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="point">The point.</param>
	/// <param name="stringFormat">The string format instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DrawValue<T>(this Graphics @this, T value, Font font, Brush brush, scoped in PointF point, StringFormat stringFormat)
		where T : notnull => @this.DrawString(value.ToString(), font, brush, point, stringFormat);

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
	public static void DrawCrossSign(this Graphics @this, Pen pen, scoped in RectangleF rectangle)
	{
		var (x, y, w, h) = rectangle;
		var p1 = new PointF(x, y + h);
		var p2 = new PointF(x + w, y);
		var p3 = new PointF(x, y);
		var p4 = new PointF(x + w, y + h);

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
	public static void DrawCapsule(this Graphics @this, Pen pen, scoped in RectangleF rectangle) => @this.DrawRoundedRectangle(pen, rectangle, 0);

	/// <summary>
	/// Fill a capsule.
	/// </summary>
	/// <param name="this">The graphics.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="rectangle">The rectangle.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FillCapsule(this Graphics @this, Brush brush, scoped in RectangleF rectangle)
		=> @this.FillRoundedRectangle(brush, rectangle, 0);

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
	public static void DrawRoundedRectangle(this Graphics @this, Pen pen, in RectangleF rectangle, float circleRadius)
	{
		if (circleRadius > Max(rectangle.Width, rectangle.Height))
		{
			throw new ArgumentOutOfRangeException(nameof(circleRadius));
		}

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

		RectangleF r1, r2, r3, r4;
		r1 = new(rectangle.X, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r2 = new(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r3 = new(rectangle.X, rectangle.Y + rectangle.Height - 2 * circleRadius, circleRadius * 2, circleRadius * 2);
		r4 = new(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y + rectangle.Height - 2 * circleRadius, circleRadius * 2, circleRadius * 2);

		using var path = new GraphicsPath();
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
	public static void FillRoundedRectangle(this Graphics @this, Brush brush, scoped in RectangleF rectangle, float circleRadius)
	{
		if (circleRadius >= Max(rectangle.Width, rectangle.Height))
		{
			throw new ArgumentException("Specified argument is greater than the value in rectangle", nameof(circleRadius));
		}

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

		RectangleF r1, r2, r3, r4;
		r1 = new(rectangle.X, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r2 = new(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y, circleRadius * 2, circleRadius * 2);
		r3 = new(rectangle.X, rectangle.Y + rectangle.Height - 2 * circleRadius, circleRadius * 2, circleRadius * 2);
		r4 = new(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y + rectangle.Height - 2 * circleRadius, circleRadius * 2, circleRadius * 2);

		using var path = new GraphicsPath();
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

	/// <summary>
	/// Fills a hollow arrow.
	/// </summary>
	/// <param name="g">The graphics instance.</param>
	/// <param name="brush">The brush.</param>
	/// <param name="center">The center point.</param>
	/// <param name="length">The length.</param>
	/// <param name="width">The width.</param>
	/// <param name="height">The height.</param>
	/// <param name="angle">The angle.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="length"/> is below 0,
	/// or argument <paramref name="width"/> or <paramref name="height"/> is below 0.
	/// </exception>
	public static void DrawHollowArrow(this Graphics g, Brush brush, PointF center, float length, float width, float height, float angle)
	{
		Argument.ThrowIfFalse(length > 0, $"Argument '{nameof(length)}' cannot be negative or 0.");
		Argument.ThrowIfFalse(width > 0, $"Argument '{nameof(width)}' cannot be negative or 0.");
		Argument.ThrowIfFalse(height > 0, $"Argument '{nameof(height)}' cannot be negative or 0.");

		var halfWidth = width / 2;
		var totalHeight = height + length;
		var heightHalf = (height + length) / 2;
		var arrowBarWidth = (width + height) / 4;
		var (x, y) = center;
		var points = new PointF[]
		{
			new(x, y - (height + length) / 2),
			new(x + halfWidth, y - (height + length) / 2 + height),
			new(x + halfWidth - (width - arrowBarWidth) / 2, y - heightHalf + height),
			new(x + halfWidth - (width - arrowBarWidth) / 2, y - heightHalf + totalHeight),
			new(x - halfWidth + (width - arrowBarWidth) / 2, y - heightHalf + totalHeight),
			new(x - halfWidth + (width - arrowBarWidth) / 2, y - heightHalf + height),
			new(x - halfWidth, y - heightHalf + height),
		};

		// Rotating.
		var oldMatrix = g.Transform;
		using var newMatrix = g.Transform.Clone();
		newMatrix.RotateAt(angle, center);

		g.Transform = newMatrix;
		g.FillPolygon(brush, points);
		g.Transform = oldMatrix;
	}
}
