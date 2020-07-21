using System.Diagnostics;
using DPoint = System.Drawing.Point;
using DPointF = System.Drawing.PointF;
using WPoint = System.Windows.Point;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="WPoint"/>, <see cref="DPoint"/>
	/// and <see cref="DPointF"/>.
	/// </summary>
	/// <seealso cref="WPoint"/>
	/// <seealso cref="DPoint"/>
	/// <seealso cref="DPointF"/>
	[DebuggerStepThrough]
	public static class PointEx
	{
		/// <include file='.\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this DPoint @this, out int x, out int y) =>
			(x, y) = (@this.X, @this.Y);

		/// <include file='.\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this DPointF @this, out float x, out float y) =>
			(x, y) = (@this.X, @this.Y);

		/// <include file='.\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this WPoint @this, out double x, out double y) =>
			(x, y) = (@this.X, @this.Y);

		/// <summary>
		/// Convert a <see cref="DPoint"/> to <see cref="WPoint"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		public static WPoint ToWPoint(this DPoint @this) => new WPoint(@this.X, @this.Y);

		/// <summary>
		/// Convert a <see cref="WPoint"/> to <see cref="DPoint"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		public static DPoint ToDPoint(this WPoint @this) => new DPoint((int)@this.X, (int)@this.Y);

		/// <summary>
		/// Convert a <see cref="WPoint"/> to <see cref="DPointF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		public static DPointF ToDPointF(this WPoint @this) => new DPointF((float)@this.X, (float)@this.Y);

		/// <summary>
		/// To truncate the point.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to truncate.</param>
		/// <returns>The result.</returns>
		public static WPoint Truncate(this WPoint @this) => new WPoint((int)@this.X, (int)@this.Y);

		/// <summary>
		/// To truncate the point.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The point to truncate.</param>
		/// <returns>The result.</returns>
		public static DPoint Truncate(this DPointF @this) => new DPoint((int)@this.X, (int)@this.Y);
	}
}
