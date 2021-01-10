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
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this Size @this, out int width, out int height)
		{
			width = @this.Width;
			height = @this.Height;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this SizeF @this, out float width, out float height)
		{
			width = @this.Width;
			height = @this.Height;
		}

		/// <summary>
		/// To truncate the size.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The size.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Size Truncate(this SizeF @this) => new((int)@this.Width, (int)@this.Height);
	}
}
