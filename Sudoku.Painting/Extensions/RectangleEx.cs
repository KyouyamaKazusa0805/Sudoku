using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="RectangleF"/>.
	/// </summary>
	/// <seealso cref="Rect"/>
	public static class RectangleEx
	{
		/// <summary>
		/// Zoom in or out the rectangle by the specified offset.
		/// If the offset is positive, the rectangle will be larger; otherwise, smaller.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The rectangle.</param>
		/// <param name="offset">The offset to zoom in or out.</param>
		/// <returns>The new rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RectangleF Zoom(this in RectangleF @this, float offset)
		{
			var result = @this;
			result.X -= offset;
			result.Y -= offset;
			result.Width += offset * 2;
			result.Height += offset * 2;
			return result;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The rectangle.</param>
		/// <param name="point">(<see langword="out"/> parameter) The point.</param>
		/// <param name="size">(<see langword="out"/> parameter) The size.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this RectangleF @this, out PointF point, out SizeF size)
		{
			point = new(@this.X, @this.Y);
			size = new(@this.Size);
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The rectangle.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(
			this in RectangleF @this, out float x, out float y, out float width, out float height)
		{
			x = @this.X;
			y = @this.Y;
			width = @this.Width;
			height = @this.Height;
		}


		/// <summary>
		/// Create a <see cref="RectangleF"/> using two points left-up and right-down.
		/// </summary>
		/// <param name="leftUp">(<see langword="in"/> parameter) The left up point.</param>
		/// <param name="rightDown">(<see langword="in"/> parameter) The right down point.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RectangleF FromLeftUpAndRightDown(in PointF leftUp, in PointF rightDown) =>
			new(leftUp.X, leftUp.Y, rightDown.X - leftUp.X, rightDown.Y - leftUp.Y);
	}
}
