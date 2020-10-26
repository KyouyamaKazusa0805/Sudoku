using Sudoku.DocComments;
using WSize = System.Windows.Size;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="WSize"/>.
	/// </summary>
	/// <seealso cref="WSize"/>
	public static class WindowsSizeEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The size.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		public static void Deconstruct(this in WSize @this, out double width, out double height) =>
			(width, height) = (@this.Width, @this.Height);
	}
}
