namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with extension methods on <see cref="Control"/>.
/// </summary>
/// <seealso cref="Control"/>
public static class ControlExtensions
{
	/// <summary>
	/// Sets the property <see cref="Control.Foreground"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithForeground<TFrameworkElement>(this TFrameworkElement @this, Color foreground)
		where TFrameworkElement : Control
		=> @this.WithForeground(new SolidColorBrush(foreground));

	/// <summary>
	/// Sets the property <see cref="Control.Foreground"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithForeground<TFrameworkElement>(this TFrameworkElement @this, Brush foreground)
		where TFrameworkElement : Control
	{
		@this.Foreground = foreground;
		return @this;
	}
}
