using System.Windows;

namespace Sudoku.Forms.Extensions
{
	public static class PointEx
	{
		public static void Deconstruct(this Point @this, out double x, out double y) =>
			(x, y) = (@this.X, @this.Y);
	}
}
