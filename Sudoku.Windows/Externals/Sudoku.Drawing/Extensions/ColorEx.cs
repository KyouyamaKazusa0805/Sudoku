using System.Diagnostics;
using Sudoku.DocComments;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="DColor"/> and <see cref="WColor"/>.
	/// </summary>
	/// <seealso cref="DColor"/>
	/// <seealso cref="WColor"/>
	[DebuggerStepThrough]
	public static class ColorEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <param name="a">(<see langword="out"/> parameter) The alpha value.</param>
		/// <param name="r">(<see langword="out"/> parameter) The red value.</param>
		/// <param name="g">(<see langword="out"/> parameter) The green value.</param>
		/// <param name="b">(<see langword="out"/> parameter) The blue value.</param>
		public static void Deconstruct(this DColor @this, out int a, out int r, out int g, out int b) =>
			(a, r, g, b) = (@this.A, @this.R, @this.G, @this.B);

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <param name="a">(<see langword="out"/> parameter) The alpha value.</param>
		/// <param name="r">(<see langword="out"/> parameter) The red value.</param>
		/// <param name="g">(<see langword="out"/> parameter) The green value.</param>
		/// <param name="b">(<see langword="out"/> parameter) The blue value.</param>
		public static void Deconstruct(this WColor @this, out byte a, out byte r, out byte g, out byte b) =>
			(a, r, g, b) = (@this.A, @this.R, @this.G, @this.B);

		/// <summary>
		/// Convert <see cref="WColor"/> to <see cref="DColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static DColor ToDColor(this WColor @this) => DColor.FromArgb(@this.A, @this.R, @this.G, @this.B);

		/// <summary>
		/// Convert <see cref="DColor"/> to <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static WColor ToWColor(this DColor @this) => WColor.FromArgb(@this.A, @this.R, @this.G, @this.B);
	}
}
