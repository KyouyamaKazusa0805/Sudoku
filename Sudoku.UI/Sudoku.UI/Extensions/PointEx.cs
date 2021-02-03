using System.Runtime.CompilerServices;
using DPointF = System.Drawing.PointF;
using WPoint = Windows.Foundation.Point;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="WPoint"/>.
	/// </summary>
	/// <seealso cref="WPoint"/>
	public static class PointEx
	{
		/// <summary>
		/// Converts a <see cref="WPoint"/> instance to <see cref="DPointF"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The point instance.</param>
		/// <returns>The <see cref="DPointF"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DPointF ToDPointF(this in WPoint @this) => new((float)@this.X, (float)@this.Y);
	}
}
