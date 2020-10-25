using System.Drawing;
using Sudoku.DocComments;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Rectangle"/> and <see cref="RectangleF"/>.
	/// </summary>
	/// <seealso cref="Rectangle"/>
	/// <seealso cref="RectangleF"/>
	public static class RectangleEx
	{
		/// <summary>
		/// Create an instance with two points.
		/// </summary>
		/// <param name="leftUp">The left up point.</param>
		/// <param name="rightDown">The right down point.</param>
		/// <returns>The rectangle.</returns>
		public static Rectangle CreateInstance(Point leftUp, Point rightDown) =>
			new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);

		/// <summary>
		/// Create an instance with two points.
		/// </summary>
		/// <param name="leftUp">The left up point.</param>
		/// <param name="rightDown">The right down point.</param>
		/// <returns>The rectangle.</returns>
		public static RectangleF CreateInstance(PointF leftUp, PointF rightDown) =>
			new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);

		/// <summary>
		/// Zoom in or out the rectangle by the specified offset.
		/// If the offset is positive, the rectangle will be larger; otherwise, smaller.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The rectangle.</param>
		/// <param name="offset">The offset to zoom in or out.</param>
		/// <returns>The new rectangle.</returns>
		public static RectangleF Zoom(this RectangleF @this, float offset)
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
		/// <param name="this">(<see langword="this"/> parameter) The rectangle.</param>
		/// <returns>The result.</returns>
		public static Rectangle Truncate(this RectangleF @this) =>
			new((int)@this.X, (int)@this.Y, (int)@this.Width, (int)@this.Height);

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The rectangle.</param>
		/// <param name="point">(<see langword="out"/> parameter) The point.</param>
		/// <param name="size">(<see langword="out"/> parameter) The size.</param>
		public static void Deconstruct(this RectangleF @this, out PointF point, out SizeF size) =>
			(point, size) = (new(@this.X, @this.Y), new(@this.Size));

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The rectangle.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(
			this RectangleF @this, out float x, out float y, out float width, out float height) =>
			(x, y, width, height) = (@this.X, @this.Y, @this.Width, @this.Height);
	}
}
