using System.Diagnostics;
using System.Drawing;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Point"/>.
	/// </summary>
	/// <seealso cref="Point"/>
	[DebuggerStepThrough]
	public static class PointEx
	{
		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this Point @this, out int x, out int y) =>
			(x, y) = (@this.X, @this.Y);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		public static void Deconstruct(this PointF @this, out float x, out float y) =>
			(x, y) = (@this.X, @this.Y);
	}
}
