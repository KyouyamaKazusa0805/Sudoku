using System.Drawing;
using Emgu.CV.Structure;
using Sudoku.DocComments;

namespace Sudoku.Recognition.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="RotatedRect"/>.
	/// </summary>
	/// <seealso cref="RotatedRect"/>
	public static class RotateRectEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The rectangle.</param>
		/// <param name="center">The center.</param>
		/// <param name="size">The size.</param>
		public static void Deconstruct(this RotatedRect @this, out PointF center, out SizeF size)
		{
			center = @this.Center;
			size = @this.Size;
		}
	}
}
