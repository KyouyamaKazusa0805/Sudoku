namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="Rectangle"/>.
/// </summary>
/// <seealso cref="Rectangle"/>
public static class RectangleExtensions
{
	/// <summary>
	/// Sets the property <see cref="Shape.Fill"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle WithFill(this Rectangle @this, Brush brush)
	{
		@this.Fill = brush;
		return @this;
	}
}
