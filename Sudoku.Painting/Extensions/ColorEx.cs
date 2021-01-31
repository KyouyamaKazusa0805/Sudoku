using Windows.UI;
using Sudoku.DocComments;
using System.Runtime.CompilerServices;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Color"/>.
	/// </summary>
	/// <seealso cref="Color"/>
	public static class ColorEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The color instance.</param>
		/// <param name="a">(<see langword="out"/> parameter) The A value.</param>
		/// <param name="r">(<see langword="out"/> parameter) The R value.</param>
		/// <param name="g">(<see langword="out"/> parameter) The G value.</param>
		/// <param name="b">(<see langword="out"/> parameter) The B value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in Color @this, out byte a, out byte r, out byte g, out byte b)
		{
			a = @this.A;
			r = @this.R;
			g = @this.G;
			b = @this.B;
		}
	}
}
