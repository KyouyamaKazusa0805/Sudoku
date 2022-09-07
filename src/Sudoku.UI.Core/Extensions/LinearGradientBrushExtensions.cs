namespace Microsoft.UI.Xaml.Media;

/// <summary>
/// Provides with extension methods on <see cref="LinearGradientBrush"/>.
/// </summary>
/// <seealso cref="LinearGradientBrush"/>
public static class LinearGradientBrushExtensions
{
	/// <summary>
	/// Sets the property <see cref="LinearGradientBrush.StartPoint"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinearGradientBrush WithStartPoint(this LinearGradientBrush @this, double x, double y)
		=> @this.WithStartPoint(new(x, y));

	/// <summary>
	/// Sets the property <see cref="LinearGradientBrush.StartPoint"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinearGradientBrush WithStartPoint(this LinearGradientBrush @this, Point startPoint)
	{
		@this.StartPoint = startPoint;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="LinearGradientBrush.EndPoint"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinearGradientBrush WithEndPoint(this LinearGradientBrush @this, double x, double y)
		=> @this.WithEndPoint(new(x, y));

	/// <summary>
	/// Sets the property <see cref="LinearGradientBrush.EndPoint"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinearGradientBrush WithEndPoint(this LinearGradientBrush @this, Point endPoint)
	{
		@this.StartPoint = endPoint;
		return @this;
	}
}
