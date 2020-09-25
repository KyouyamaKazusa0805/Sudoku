using System.Runtime.CompilerServices;
using System.Windows;
using DSize = System.Drawing.Size;
using DSizeF = System.Drawing.SizeF;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Size"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	public static class WindowsSizeEx
	{
		/// <summary>
		/// Convert the current size instance to another instance of type <see cref="DSizeF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current size.</param>
		/// <returns>The another instance of type <see cref="DSizeF"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DSizeF ToDSizeF(this Size @this) => new((float)@this.Width, (float)@this.Height);

		/// <summary>
		/// Convert the current size instance to another instance of type <see cref="DSizeF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current size.</param>
		/// <returns>The another instance of type <see cref="DSizeF"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DSize ToDSize(this Size @this) => new((int)@this.Width, (int)@this.Height);
	}
}
