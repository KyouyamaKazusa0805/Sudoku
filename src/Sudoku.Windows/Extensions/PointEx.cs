namespace Sudoku.Windows.Extensions;

/// <summary>
/// Provides extension methods on <see cref="WPoint"/>, <see cref="DPoint"/>
/// and <see cref="DPointF"/>.
/// </summary>
/// <seealso cref="WPoint"/>
/// <seealso cref="DPoint"/>
/// <seealso cref="DPointF"/>
public static partial class PointEx
{
	/// <summary>
	/// Convert a <see cref="DPoint"/> to <see cref="WPoint"/>.
	/// </summary>
	/// <param name="this">The point to convert.</param>
	/// <returns>The result of conversion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WPoint ToWPoint(this in DPoint @this) => new(@this.X, @this.Y);

	/// <summary>
	/// Convert a <see cref="WPoint"/> to <see cref="DPoint"/>.
	/// </summary>
	/// <param name="this">The point to convert.</param>
	/// <returns>The result of conversion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DPoint ToDPoint(this in WPoint @this) => new((int)@this.X, (int)@this.Y);

	/// <summary>
	/// Convert a <see cref="WPoint"/> to <see cref="DPointF"/>.
	/// </summary>
	/// <param name="this">The point to convert.</param>
	/// <returns>The result of conversion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DPointF ToDPointF(this in WPoint @this) => new((float)@this.X, (float)@this.Y);

	/// <summary>
	/// To truncate the point.
	/// </summary>
	/// <param name="this">The point to truncate.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WPoint Truncate(this in WPoint @this) => new((int)@this.X, (int)@this.Y);
}
