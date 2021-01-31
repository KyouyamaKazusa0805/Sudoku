using System.Runtime.CompilerServices;
using Windows.Foundation;
using Sudoku.DocComments;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Point"/>.
	/// </summary>
	/// <seealso cref="Point"/>
	public static class PointEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The point instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The X value.</param>
		/// <param name="height">(<see langword="out"/> parameter) The Y value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in Point @this, out double x, out double y)
		{
			x = @this.X;
			y = @this.Y;
		}

		/// <summary>
		/// Get a new <see cref="Point"/> instance created by the original one, with the specified offset
		/// added into the propeties <see cref="Point.X"/> and <see cref="Point.Y"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point.</param>
		/// <param name="offset">The offset.</param>
		/// <returns>The result point.</returns>
		/// <seealso cref="Point.X"/>
		/// <seealso cref="Point.Y"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Point WithOffset(this in Point @this, double offset) =>
			new(@this.X + offset, @this.Y + offset);

		/// <summary>
		/// Get a new <see cref="Point"/> instance created by the original one, with the specified offset
		/// added into the propeties <see cref="Point.X"/> and <see cref="Point.Y"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point.</param>
		/// <param name="xOffset">The X offset.</param>
		/// <param name="yOffset">The Y offset.</param>
		/// <returns>The result point.</returns>
		/// <seealso cref="Point.X"/>
		/// <seealso cref="Point.Y"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Point WithOffset(this in Point @this, double xOffset, double yOffset) =>
			new(@this.X + xOffset, @this.Y + yOffset);
	}
}
