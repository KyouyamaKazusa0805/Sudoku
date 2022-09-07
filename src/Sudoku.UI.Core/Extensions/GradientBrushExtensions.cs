namespace Microsoft.UI.Xaml.Media;

/// <summary>
/// Provides with extension methods on <see cref="GradientBrush"/>.
/// </summary>
/// <seealso cref="GradientBrush"/>
public static class GradientBrushExtensions
{
	/// <summary>
	/// Sets the property <see cref="GradientBrush.GradientStops"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TGradientBrush WithGradientStops<TGradientBrush>(this TGradientBrush @this, GradientStopCollection gradientStops)
		where TGradientBrush : GradientBrush
	{
		@this.GradientStops = gradientStops;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="GradientBrush.GradientStops"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TGradientBrush WithGradientStops<TGradientBrush>(this TGradientBrush @this, params GradientStop[] gradientStops)
		where TGradientBrush : GradientBrush
	{
		var gsc = new GradientStopCollection();
		gsc.AddRange(gradientStops);

		@this.GradientStops = gsc;
		return @this;
	}
}
