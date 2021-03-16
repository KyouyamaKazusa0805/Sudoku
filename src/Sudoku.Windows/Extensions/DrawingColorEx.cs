using System.Runtime.CompilerServices;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="DColor"/>.
	/// </summary>
	/// <seealso cref="DColor"/>
	public static class DrawingColorEx
	{
		/// <summary>
		/// Convert <see cref="DColor"/> to <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">The color.</param>
		/// <returns>The target color.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WColor ToWColor(this in DColor @this) =>
			WColor.FromArgb(@this.A, @this.R, @this.G, @this.B);
	}
}
