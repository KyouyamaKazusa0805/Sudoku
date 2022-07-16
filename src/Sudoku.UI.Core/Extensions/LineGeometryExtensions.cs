namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="LineGeometry"/>.
/// </summary>
/// <seealso cref="LineGeometry"/>
public static class LineGeometryExtensions
{
	/// <summary>
	/// Gets the customized arrow cap geometry instances that can be used
	/// as the property <see cref="GeometryGroup.Children"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Geometry[] WithCustomizedArrowCap(this LineGeometry @this)
	{
		var pt1 = @this.StartPoint;
		var pt2 = @this.EndPoint;
		double headlen = 10;
		double theta = 30;
		double arrowX, arrowY;
		double angle = Atan2(pt1.Y - pt2.Y, pt1.X - pt2.X) * 180 / PI;
		double angle1 = (angle + theta) * PI / 180;
		double angle2 = (angle - theta) * PI / 180;
		double topX = headlen * Cos(angle1);
		double topY = headlen * Sin(angle1);
		double botX = headlen * Cos(angle2);
		double botY = headlen * Sin(angle2);

		arrowX = pt2.X + topX;
		arrowY = pt2.Y + topY;
		var a = new LineGeometry().WithPoints(new(arrowX, arrowY), pt2);

		arrowX = pt2.X + botX;
		arrowY = pt2.Y + botY;
		var b = new LineGeometry().WithPoints(new(arrowX, arrowY), pt2);

		return new[] { @this, a, b };
	}

	/// <summary>
	/// Sets properties <see cref="LineGeometry.StartPoint"/> and <see cref="LineGeometry.EndPoint"/>
	/// with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LineGeometry WithPoints(this LineGeometry @this, Point startPoint, Point endPoint)
	{
		@this.StartPoint = startPoint;
		@this.EndPoint = endPoint;
		return @this;
	}
}
