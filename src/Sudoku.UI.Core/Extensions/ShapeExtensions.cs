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
	public static TShape WithFill<TShape>(this TShape @this, Color fill) where TShape : Shape
		=> @this.WithFill(new SolidColorBrush(fill));

	/// <summary>
	/// Sets the property <see cref="Shape.Fill"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TShape WithFill<TShape>(this TShape @this, Brush fill) where TShape : Shape
	{
		@this.Fill = fill;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Shape.Stroke"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TShape WithStroke<TShape>(this TShape @this, Color stroke) where TShape : Shape
		=> @this.WithStroke(new SolidColorBrush(stroke));

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
