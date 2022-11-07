namespace Windows.Foundation;

/// <summary>
/// Provides with extension methods on <see cref="Point"/>.
/// </summary>
/// <seealso cref="Point"/>
public static partial class PointExtensions
{
	/// <summary>
	/// Gets the distance between the two points, starting with the current point, and ending with the specified point.
	/// </summary>
	/// <param name="this">The current point.</param>
	/// <param name="other">The other point.</param>
	/// <returns>The distance of the two points.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DistanceTo(this Point @this, Point other)
		=> Sqrt((@this.X - other.X) * (@this.X - other.X) + (@this.Y - other.Y) * (@this.Y - other.Y));

	[GeneratedDeconstruction]
	public static partial void Deconstruct(this scoped in Point @this, out double x, out double y);
}
