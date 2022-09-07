namespace Microsoft.UI.Xaml.Media;

/// <summary>
/// Provides with extension methods on <see cref="GradientStop"/>.
/// </summary>
/// <seealso cref="GradientStop"/>
public static class GradientStopExtensions
{
	/// <summary>
	/// Sets the property <see cref="GradientStop.Color"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GradientStop WithColor(this GradientStop @this, Color color)
	{
		@this.Color = color;
		return @this;
	}
}
