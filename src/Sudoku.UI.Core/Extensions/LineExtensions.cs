namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="Line"/>.
/// </summary>
/// <seealso cref="Line"/>
public static class LineExtensions
{
	/// <summary>
	/// Sets properties <see cref="Line.X1"/> and <see cref="Line.Y1"/> with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line WithStartPoint(this Line @this, double startPointX, double startPointY)
	{
		(@this.X1, @this.X2) = (startPointX, startPointY);
		return @this;
	}

	/// <summary>
	/// Sets properties <see cref="Line.X1"/> and <see cref="Line.Y1"/> with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line WithStartPoint(this Line @this, Point startPoint)
		=> @this.WithStartPoint(startPoint.X, startPoint.Y);

	/// <summary>
	/// Sets properties <see cref="Line.X2"/> and <see cref="Line.Y2"/> with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line WithEndPoint(this Line @this, double endPointX, double endPointY)
	{
		(@this.X1, @this.X2) = (endPointX, endPointY);
		return @this;
	}

	/// <summary>
	/// Sets properties <see cref="Line.X2"/> and <see cref="Line.Y2"/> with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line WithEndPoint(this Line @this, Point endPoint) => @this.WithEndPoint(endPoint.X, endPoint.Y);

	/// <summary>
	/// Sets properties <see cref="Line.X1"/>, <see cref="Line.X2"/>, <see cref="Line.Y1"/> and <see cref="Line.Y2"/>
	/// with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line WithPoints(this Line @this, Point point1, Point point2)
	{
		((@this.X1, @this.Y1), (@this.X2, @this.Y2)) = (point1, point2);
		return @this;
	}
}
