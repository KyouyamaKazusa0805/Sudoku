using Windows.Foundation;
using Sudoku.DocComments;
using System.Runtime.CompilerServices;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Size"/>
	/// </summary>
	/// <seealso cref="Size"/>
	public static class SizeEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in Size @this, out double width, out double height)
		{
			width = @this.Width;
			height = @this.Height;
		}
	}
}
