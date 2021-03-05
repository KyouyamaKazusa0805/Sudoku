using System.Runtime.CompilerServices;
using Sudoku.DocComments;
using DSize = System.Drawing.Size;
using DSizeF = System.Drawing.SizeF;
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in WSize @this, out double width, out double height)
		{
			width = @this.Width;
			height = @this.Height;
		}

		/// <summary>
		/// Convert the current size instance to another instance of type <see cref="DSizeF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current size.</param>
		/// <returns>The another instance of type <see cref="DSizeF"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DSizeF ToDSizeF(this WSize @this) => new((float)@this.Width, (float)@this.Height);

		/// <summary>
		/// Convert the current size instance to another instance of type <see cref="DSizeF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current size.</param>
		/// <returns>The another instance of type <see cref="DSizeF"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DSize ToDSize(this WSize @this) => new((int)@this.Width, (int)@this.Height);
	}
}
