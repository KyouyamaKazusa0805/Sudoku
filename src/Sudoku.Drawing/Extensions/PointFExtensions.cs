namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="PointF"/>.
/// </summary>
/// <seealso cref="PointF"/>
internal static class PointFExtensions
{
	/// <summary>
	/// To truncate the point.
	/// </summary>
	/// <param name="this">The point to truncate.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Truncate(this in PointF @this) => new((int)@this.X, (int)@this.Y);

	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.X"/> and <see cref="PointF.Y"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="offset">The offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.X"/>
	/// <seealso cref="PointF.Y"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF WithOffset(this in PointF @this, float offset) => new(@this.X + offset, @this.Y + offset);

	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.X"/> and <see cref="PointF.Y"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="xOffset">The X offset.</param>
	/// <param name="yOffset">The Y offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.X"/>
	/// <seealso cref="PointF.Y"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF WithOffset(this in PointF @this, float xOffset, float yOffset) =>
		new(@this.X + xOffset, @this.Y + yOffset);

	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.X"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="xOffset">The X offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.X"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF WithX(this in PointF @this, float xOffset) => new(@this.X + xOffset, @this.Y);

	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.Y"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="yOffset">The Y offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.Y"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF WithY(this in PointF @this, float yOffset) => new(@this.X, @this.Y + yOffset);
}