using WPoint = Windows.Foundation.Point;
using DPointF = System.Drawing.PointF;
using System.Runtime.CompilerServices;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="WPoint"/>.
	/// </summary>
	/// <seealso cref="WPoint"/>
	public static class PointEx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DPointF ToDPointF(this in WPoint @this) => new((float)@this.X, (float)@this.Y);
	}
}
