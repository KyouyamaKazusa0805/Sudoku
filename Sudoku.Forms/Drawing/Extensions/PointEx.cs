using System.Diagnostics;
using d = System.Drawing;
using w = System.Windows;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="w.Point"/>, <see cref="d.Point"/>
	/// and <see cref="d.PointF"/>.
	/// </summary>
	/// <seealso cref="w.Point"/>
	/// <seealso cref="d.Point"/>
	/// <seealso cref="d.PointF"/>
	[DebuggerStepThrough]
	public static class PointEx
	{
		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this d::Point @this, out int x, out int y) =>
			(x, y) = (@this.X, @this.Y);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this d::PointF @this, out float x, out float y) =>
			(x, y) = (@this.X, @this.Y);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this w::Point @this, out double x, out double y) =>
			(x, y) = (@this.X, @this.Y);

		/// <summary>
		/// Convert a <see cref="d.Point"/> to <see cref="w.Point"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		public static w::Point ToWPoint(this d::Point @this) =>
			new w::Point(@this.X, @this.Y);

		/// <summary>
		/// Convert a <see cref="w.Point"/> to <see cref="d.Point"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		public static d::Point ToDPoint(this w::Point @this) =>
			new d::Point((int)@this.X, (int)@this.Y);

		/// <summary>
		/// Convert a <see cref="w.Point"/> to <see cref="d.PointF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		public static d::PointF ToDPointF(this w::Point @this) =>
			new d::PointF((float)@this.X, (float)@this.Y);

		/// <summary>
		/// To truncate the point.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to truncate.</param>
		/// <returns>The result.</returns>
		public static w::Point Truncate(this w::Point @this) =>
			new w::Point((int)@this.X, (int)@this.Y);
	}
}
