namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="Rectangle"/>.
/// </summary>
/// <seealso cref="Rectangle"/>
public static class RectangleExtensions
{
	/// <summary>
	/// Sets the properties <see cref="Rectangle.RadiusX"/> and <see cref="Rectangle.RadiusY"/>
	/// with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle WithRadius(this Rectangle @this, double radius) => @this.WithRadius(radius, radius);

	/// <summary>
	/// Sets the properties <see cref="Rectangle.RadiusX"/> and <see cref="Rectangle.RadiusY"/>
	/// with the specified values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle WithRadius(this Rectangle @this, double radiusX, double radiusY)
	{
		@this.RadiusX = radiusX;
		@this.RadiusY = radiusY;
		return @this;
	}
}
