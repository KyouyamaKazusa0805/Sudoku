namespace Windows.Foundation;

/// <summary>
/// Provides extension methods on <see cref="PointF"/>.
/// </summary>
/// <seealso cref="PointF"/>
internal static class PointExtensions
{
	/// <summary>
	/// Compares two <see cref="Point"/>s, to determine whether the current point is nearly equal to another one.
	/// </summary>
	/// <param name="this">The current <see cref="Point"/> instance.</param>
	/// <param name="other">Another <see cref="Point"/> instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NearlyEquals(this in Point @this, in Point other) =>
		@this.X.NearlyEquals(other.X) && @this.Y.NearlyEquals(other.Y);

	/// <summary>
	/// Compares two <see cref="Point"/>s, to determine whether the current point is nearly equal to another one,
	/// using the specified epsilon to compare.
	/// </summary>
	/// <param name="this">The current <see cref="Point"/> instance.</param>
	/// <param name="other">Another <see cref="Point"/> instance.</param>
	/// <param name="epsilon">Indicates the epsilon to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NearlyEquals(this in Point @this, in Point other, double epsilon) =>
		@this.X.NearlyEquals(other.X, epsilon) && @this.Y.NearlyEquals(other.Y, epsilon);

	/// <summary>
	/// To truncate the point.
	/// </summary>
	/// <param name="this">The point to truncate.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Truncate(this in Point @this) => new((int)@this.X, (int)@this.Y);

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
	public static Point WithOffset(this in Point @this, double offset) => new(@this.X + offset, @this.Y + offset);

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
	public static Point WithOffset(this in Point @this, double xOffset, double yOffset) =>
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
	public static Point WithX(this in Point @this, double xOffset) => new(@this.X + xOffset, @this.Y);

	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.Y"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="yOffset">The Y offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.Y"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point WithY(this in Point @this, double yOffset) => new(@this.X, @this.Y + yOffset);
}