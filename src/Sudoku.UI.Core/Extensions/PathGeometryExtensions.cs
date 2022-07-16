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
}
