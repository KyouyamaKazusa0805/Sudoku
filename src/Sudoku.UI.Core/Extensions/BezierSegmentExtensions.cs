namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="BezierSegment"/>.
/// </summary>
/// <seealso cref="BezierSegment"/>
public static class BezierSegmentExtensions
{
	/// <summary>
	/// Sets properties <see cref="BezierSegment.Point1"/> and <see cref="BezierSegment.Point2"/>
	/// with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BezierSegment WithInterimPoints(
		this BezierSegment @this, double pt1x, double pt1y, double pt2x, double pt2y)
		=> @this.WithInterimPoints(new(pt1x, pt1y), new(pt2x, pt2y));

	/// <summary>
	/// Sets properties <see cref="BezierSegment.Point1"/> and <see cref="BezierSegment.Point2"/>
	/// with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BezierSegment WithInterimPoints(this BezierSegment @this, Point point1, Point point2)
	{
		@this.Point1 = point1;
		@this.Point2 = point2;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="BezierSegment.Point3"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BezierSegment WithEndPoint(this BezierSegment @this, Point endPoint)
	{
		@this.Point3 = endPoint;
		return @this;
	}
}
