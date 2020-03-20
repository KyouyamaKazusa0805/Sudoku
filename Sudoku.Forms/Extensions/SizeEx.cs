using System.Windows;

namespace Sudoku.Forms.Extensions
{
	public static class SizeEx
	{
		public static void Deconstruct(this Size @this, out double width, out double height) =>
			(width, height) = (@this.Width, @this.Height);
	}
}
