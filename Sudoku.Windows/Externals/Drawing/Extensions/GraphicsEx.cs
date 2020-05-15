using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sudoku.Drawing.Extensions
{
	public static class GraphicsEx
	{
		/// <summary>
		/// Draw a rounded rectangle.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The graphics.</param>
		/// <param name="pen">The pen.</param>
		/// <param name="rectangle">The rectangle to draw.</param>
		/// <param name="circleRadius">The radius of each vertex.</param>
		/// <exception cref="ArgumentException">
		/// Throws when <paramref name="circleRadius"/> is greater than the value in
		/// <paramref name="rectangle"/>.
		/// </exception>
		public static void DrawRoundedRectangle(
			this Graphics @this, Pen pen, RectangleF rectangle, float circleRadius)
		{
			if (circleRadius > Math.Max(rectangle.Width, rectangle.Height))
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
			r1 = new RectangleF(rectangle.X, rectangle.Y, circleRadius * 2, circleRadius * 2);
			r2 = new RectangleF(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y,
				circleRadius * 2, circleRadius * 2);
			r3 = new RectangleF(rectangle.X, rectangle.Y + rectangle.Height - 2 * circleRadius,
				circleRadius * 2, circleRadius * 2);
			r4 = new RectangleF(rectangle.X + rectangle.Width - 2 * circleRadius,
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
		/// <param name="this">(<see langword="this"/> parameter) The graphics.</param>
		/// <param name="brush">The brush.</param>
		/// <param name="rectangle">The rectangle to fill.</param>
		/// <param name="circleRadius">The radius of each vertex.</param>
		/// <exception cref="ArgumentException">
		/// Throws when <paramref name="circleRadius"/> is greater than the value in
		/// <paramref name="rectangle"/>.
		/// </exception>
		public static void FillRoundedRectangle(
			this Graphics @this, Brush brush, RectangleF rectangle, float circleRadius)
		{
			if (circleRadius >= Math.Max(rectangle.Width, rectangle.Height))
			{
				throw new ArgumentException(
					"Specified argument is greater than the value in rectangle", nameof(circleRadius));
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
			r1 = new RectangleF(rectangle.X, rectangle.Y, circleRadius * 2, circleRadius * 2);
			r2 = new RectangleF(rectangle.X + rectangle.Width - 2 * circleRadius, rectangle.Y,
				circleRadius * 2, circleRadius * 2);
			r3 = new RectangleF(rectangle.X, rectangle.Y + rectangle.Height - 2 * circleRadius,
				circleRadius * 2, circleRadius * 2);
			r4 = new RectangleF(rectangle.X + rectangle.Width - 2 * circleRadius,
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

			@this.FillPath(brush, path);
		}
	}
}
