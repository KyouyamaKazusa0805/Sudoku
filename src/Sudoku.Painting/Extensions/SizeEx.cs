using System.Drawing;
using System.Runtime.CompilerServices;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Size"/> and <see cref="SizeF"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	/// <seealso cref="SizeF"/>
	public static partial class SizeEx
	{
		/// <summary>
		/// To truncate the size.
		/// </summary>
		/// <param name="this">The size.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Size Truncate(this in SizeF @this) => new((int)@this.Width, (int)@this.Height);
	}
}
