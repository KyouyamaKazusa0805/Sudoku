namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="Shape"/>.
/// </summary>
/// <seealso cref="Shape"/>
public static class ShapeExtensions
{
	/// <summary>
	/// Sets the property <see cref="Shape.Fill"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TShape WithFill<TShape>(this TShape @this, Brush brush) where TShape : Shape
	{
		@this.Fill = brush;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Shape.Stroke"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TShape WithStroke<TShape>(this TShape @this, Brush stroke) where TShape : Shape
	{
		@this.Stroke = stroke;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Shape.StrokeThickness"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TShape WithStrokeThickness<TShape>(this TShape @this, double strokeThickness) where TShape : Shape
	{
		@this.StrokeThickness = strokeThickness;
		return @this;
	}
}
