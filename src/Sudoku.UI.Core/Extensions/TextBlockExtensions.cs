namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with extension methods on <see cref="TextBlock"/>.
/// </summary>
/// <seealso cref="TextBlock"/>
public static class TextBlockExtensions
{
	/// <summary>
	/// Sets the property <see cref="TextBlock.Foreground"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithForeground(this TextBlock @this, Color foreground)
		=> @this.WithForeground(new SolidColorBrush(foreground));

	/// <summary>
	/// Sets the property <see cref="TextBlock.Foreground"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithForeground(this TextBlock @this, Brush foreground)
	{
		@this.Foreground = foreground;
		return @this;
	}
}