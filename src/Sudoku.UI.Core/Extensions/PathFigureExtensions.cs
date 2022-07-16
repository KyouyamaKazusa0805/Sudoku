namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="PathFigure"/>.
/// </summary>
/// <seealso cref="PathFigure"/>
public static class PathFigureExtensions
{
	/// <summary>
	/// Sets the property <see cref="PathFigure.StartPoint"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathFigure WithStartPoint(this PathFigure @this, Point startPoint)
	{
		@this.StartPoint = startPoint;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="PathFigure.IsClosed"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathFigure WithIsClosed(this PathFigure @this, bool isClosed)
	{
		@this.IsClosed = isClosed;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="PathFigure.IsFilled"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathFigure WithIsFilled(this PathFigure @this, bool isFilled)
	{
		@this.IsFilled = isFilled;
		return @this;
	}

	/// <summary>
	/// Adds a new <see cref="PathSegment"/> into the <see cref="PathFigure.Segments"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathFigure AddSegment<TPathSegment>(this PathFigure @this, TPathSegment pathSegment)
		where TPathSegment : PathSegment
	{
		@this.Segments.Add(pathSegment);
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="PathFigure.Segments"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PathFigure WithSegments(this PathFigure @this, params PathSegment[] pathSegments)
	{
		var pathSegmentsCollection = new PathSegmentCollection();
		pathSegmentsCollection.AddRange(pathSegments);

		@this.Segments = pathSegmentsCollection;
		return @this;
	}
}
