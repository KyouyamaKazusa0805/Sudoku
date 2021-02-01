using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="PointF"/>.
	/// </summary>
	/// <seealso cref="PointF"/>
	public static class PointEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The point instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The X value.</param>
		/// <param name="height">(<see langword="out"/> parameter) The Y value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in PointF @this, out float x, out float y)
		{
			x = @this.X;
			y = @this.Y;
		}

		/// <summary>
		/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
		/// added into the propeties <see cref="PointF.X"/> and <see cref="PointF.Y"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point.</param>
		/// <param name="offset">The offset.</param>
		/// <returns>The result point.</returns>
		/// <seealso cref="PointF.X"/>
		/// <seealso cref="PointF.Y"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PointF WithOffset(this in PointF @this, float offset) =>
			new(@this.X + offset, @this.Y + offset);

		/// <summary>
		/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
		/// added into the propeties <see cref="PointF.X"/> and <see cref="PointF.Y"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point.</param>
		/// <param name="xOffset">The X offset.</param>
		/// <param name="yOffset">The Y offset.</param>
		/// <returns>The result point.</returns>
		/// <seealso cref="PointF.X"/>
		/// <seealso cref="PointF.Y"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PointF WithOffset(this in PointF @this, float xOffset, float yOffset) =>
			new(@this.X + xOffset, @this.Y + yOffset);
	}
}
