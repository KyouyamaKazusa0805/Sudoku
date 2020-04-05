using System.Diagnostics;
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
		/// <summary>
		/// Convert <see cref="WColor"/> to <see cref="DColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static DColor ToDColor(this WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B);

		/// <summary>
		/// Convert <see cref="DColor"/> to <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static WColor ToWColor(this DColor @this) =>
			WColor.FromArgb(@this.A, @this.R, @this.G, @this.B);
	}
}
