using System.Runtime.CompilerServices;
using Sudoku.DocComments;
using DPoint = System.Drawing.Point;
using DPointF = System.Drawing.PointF;
using WPoint = System.Windows.Point;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="WPoint"/>, <see cref="DPoint"/>
	/// and <see cref="DPointF"/>.
	/// </summary>
	/// <seealso cref="WPoint"/>
	/// <seealso cref="DPoint"/>
	/// <seealso cref="DPointF"/>
	public static class PointEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The instance.</param>
		/// <param name="x">(<see langword="out"/> parameter) The x component.</param>
		/// <param name="y">(<see langword="out"/> parameter) The y component.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in WPoint @this, out double x, out double y)
		{
			x = @this.X;
			y = @this.Y;
		}

		/// <summary>
		/// Convert a <see cref="DPoint"/> to <see cref="WPoint"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WPoint ToWPoint(this in DPoint @this) => new(@this.X, @this.Y);

		/// <summary>
		/// Convert a <see cref="WPoint"/> to <see cref="DPoint"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DPoint ToDPoint(this in WPoint @this) => new((int)@this.X, (int)@this.Y);

		/// <summary>
		/// Convert a <see cref="WPoint"/> to <see cref="DPointF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point to convert.</param>
		/// <returns>The result of conversion.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DPointF ToDPointF(this in WPoint @this) => new((float)@this.X, (float)@this.Y);

		/// <summary>
		/// To truncate the point.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point to truncate.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WPoint Truncate(this in WPoint @this) => new((int)@this.X, (int)@this.Y);
	}
}
