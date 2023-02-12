namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="PointF"/>.
/// </summary>
/// <seealso cref="PointF"/>
internal static partial class PointFExtensions
{
	/// <summary>
	/// To truncate the point.
	/// </summary>
	/// <param name="this">The point to truncate.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Truncate(this PointF @this) => new((int)@this.X, (int)@this.Y);

	[GeneratedDeconstruction]
	public static partial void Deconstruct(this PointF @this, out float x, out float y);
}
