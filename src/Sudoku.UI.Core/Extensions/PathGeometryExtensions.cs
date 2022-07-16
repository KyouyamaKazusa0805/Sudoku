namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="PathGeometry"/>.
/// </summary>
/// <seealso cref="PathGeometry"/>
public static class PathGeometryExtensions
{
	/// <summary>
	/// Adds a new <see cref="PathFigure"/> instance into the collection <see cref="PathGeometry.Figures"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathGeometry AddFigure(this PathGeometry @this, PathFigure pathFigure)
	{
		@this.Figures.Add(pathFigure);
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="PathGeometry.Figures"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathGeometry WithFigures(this PathGeometry @this, params PathFigure[] pathFigures)
	{
		var pathFiguresCollection = new PathFigureCollection();
		pathFiguresCollection.AddRange(pathFigures);

		@this.Figures = pathFiguresCollection;
		return @this;
	}

	/// <summary>
	/// Gets the customized arrow cap geometry instances that can be used
	/// as the property <see cref="GeometryGroup.Children"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Geometry[] WithCustomizedArrowCap(this PathGeometry @this, Point point1, Point point2)
	{
		var pt1 = point1;
		var pt2 = point2;
		double arrowLength = 10;
		double theta = 30;
		double arrowX, arrowY;
		double angle = Atan2(pt1.Y - pt2.Y, pt1.X - pt2.X) * 180 / PI;
		double angle1 = (angle + theta + 22.5) * PI / 180;
		double angle2 = (angle - theta + 22.5) * PI / 180;
		double topX = arrowLength * Cos(angle1);
		double topY = arrowLength * Sin(angle1);
		double bottomX = arrowLength * Cos(angle2);
		double bottomY = arrowLength * Sin(angle2);

		arrowX = pt2.X + topX;
		arrowY = pt2.Y + topY;
		var a = new LineGeometry().WithPoints(new(arrowX, arrowY), pt2);

		arrowX = pt2.X + bottomX;
		arrowY = pt2.Y + bottomY;
		var b = new LineGeometry().WithPoints(new(arrowX, arrowY), pt2);

		return new Geometry[] { @this, a, b };
	}
}
