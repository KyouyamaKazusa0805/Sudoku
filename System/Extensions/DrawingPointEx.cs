using Sudoku.DocComments;
using DPoint = System.Drawing.Point;
using DPointF = System.Drawing.PointF;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="DPoint"/> and <see cref="DPointF"/>.
	/// </summary>
	/// <seealso cref="DPoint"/>
	/// <seealso cref="DPointF"/>
	public static class DrawingPointEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this DPoint @this, out int x, out int y) => (x, y) = (@this.X, @this.Y);

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this DPointF @this, out float x, out float y) => (x, y) = (@this.X, @this.Y);

		/// <summary>
		/// To truncate the point.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to truncate.</param>
		/// <returns>The result.</returns>
		public static DPoint Truncate(this DPointF @this) => new((int)@this.X, (int)@this.Y);
	}
}
