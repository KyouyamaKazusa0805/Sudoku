using System.Diagnostics;
using System.Drawing;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Rectangle"/> and <see cref="RectangleF"/>.
	/// </summary>
	/// <seealso cref="Rectangle"/>
	/// <seealso cref="RectangleF"/>
	[DebuggerStepThrough]
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
	}
}
