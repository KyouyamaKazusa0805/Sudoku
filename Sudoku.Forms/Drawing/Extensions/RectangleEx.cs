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
			new Rectangle(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);

		/// <summary>
		/// Create an instance with two points.
		/// </summary>
		/// <param name="leftUp">The left up point.</param>
		/// <param name="rightDown">The right down point.</param>
		/// <returns>The rectangle.</returns>
		public static RectangleF CreateInstance(PointF leftUp, PointF rightDown) =>
			new RectangleF(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);
	}
}
