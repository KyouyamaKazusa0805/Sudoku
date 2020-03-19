using System.Diagnostics;
using System.Drawing;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Size"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	[DebuggerStepThrough]
	public static class SizeEx
	{
		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(this Size @this, out int width, out int height) =>
			(width, height) = (@this.Width, @this.Height);
	}
}
