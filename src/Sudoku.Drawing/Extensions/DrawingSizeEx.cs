using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Size"/> and <see cref="SizeF"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	/// <seealso cref="SizeF"/>
	public static class DrawingSizeEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The size instance.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in Size @this, out int width, out int height)
		{
			width = @this.Width;
			height = @this.Height;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The size instance.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in SizeF @this, out float width, out float height)
		{
			width = @this.Width;
			height = @this.Height;
		}

		/// <summary>
		/// To truncate the size.
		/// </summary>
		/// <param name="this">The size.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Size Truncate(this in SizeF @this) => new((int)@this.Width, (int)@this.Height);
	}
}
