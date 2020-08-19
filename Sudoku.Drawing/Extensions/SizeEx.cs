using System.Diagnostics;
using System.Drawing;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Size"/> and <see cref="SizeF"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	/// <seealso cref="SizeF"/>
	[DebuggerStepThrough]
	public static class SizeEx
	{
		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(this Size @this, out int width, out int height) =>
			(width, height) = (@this.Width, @this.Height);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(this SizeF @this, out float width, out float height) =>
			(width, height) = (@this.Width, @this.Height);

		/// <summary>
		/// To truncate the size.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The size.</param>
		/// <returns>The result.</returns>
		public static Size Truncate(this SizeF @this) => new((int)@this.Width, (int)@this.Height);
	}
}
