using System.Diagnostics;
using d = System.Drawing;
using w = System.Windows;

namespace Sudoku.Forms.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="d.Color"/> and <see cref="w.Media.Color"/>.
	/// </summary>
	/// <seealso cref="d.Color"/>
	/// <seealso cref="w.Media.Color"/>
	[DebuggerStepThrough]
	public static class ColorEx
	{
		/// <summary>
		/// Convert <see cref="w.Media.Color"/> to <see cref="d.Color"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static d::Color ToDColor(this w::Media.Color @this) =>
			d::Color.FromArgb(@this.A, @this.R, @this.G, @this.B);

		/// <summary>
		/// Convert <see cref="d.Color"/> to <see cref="w.Media.Color"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static w::Media.Color ToWColor(this d::Color @this) =>
			w::Media.Color.FromArgb(@this.A, @this.R, @this.G, @this.B);
	}
}
