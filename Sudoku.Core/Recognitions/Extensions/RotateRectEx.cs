using System.Drawing;
using Emgu.CV.Structure;
using Sudoku.DocComments;

namespace Sudoku.Recognitions.Extensions
{
	public static class RotateRectEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The rectangle.</param>
		/// <param name="center">(<see langword="out"/> parameter) The center.</param>
		/// <param name="size">(<see langword="out"/> parameter) The size.</param>
		public static void Deconstruct(this RotatedRect @this, out PointF center, out SizeF size) =>
			(center, size) = (@this.Center, @this.Size);
	}
}
