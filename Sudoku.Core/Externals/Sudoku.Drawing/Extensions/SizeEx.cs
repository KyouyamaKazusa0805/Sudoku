using System.Drawing;
using Sudoku.DocComments;

namespace Sudoku.Drawing.Extensions
{
	public static class SizeEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(this SizeF @this, out float width, out float height) =>
			(width, height) = (@this.Width, @this.Height);
	}
}
