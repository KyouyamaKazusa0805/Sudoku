using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Sudoku.DocComments;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Image"/>.
	/// </summary>
	/// <seealso cref="Image"/>
	public static class ImageEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The image.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this Image @this, out double width, out double height)
		{
			width = @this.Width;
			height = @this.Height;
		}
	}
}
