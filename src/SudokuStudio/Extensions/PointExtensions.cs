namespace Windows.Foundation;

/// <summary>
/// Provides with extension methods on <see cref="Point"/>.
/// </summary>
/// <seealso cref="Point"/>
public static class PointExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Point @this, out double x, out double y) => (x, y) = (@this.X, @this.Y);

	/// <summary>
	/// Gets the distance between the two points, starting with the current point, and ending with the specified point.
	/// </summary>
	/// <param name="this">The current point.</param>
	/// <param name="other">The other point.</param>
	/// <returns>The distance of the two points.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DistanceTo(this Point @this, Point other)
		=> Sqrt((@this.X - other.X) * (@this.X - other.X) + (@this.Y - other.Y) * (@this.Y - other.Y));
}
