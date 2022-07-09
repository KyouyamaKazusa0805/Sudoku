namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with extension methods on <see cref="ContentControl"/>.
/// </summary>
/// <seealso cref="ContentControl"/>
public static class ContentControlExtensions
{
	/// <summary>
	/// Sets the property <see cref="ContentControl.Content"/> with the specified value.
	/// </summary>
	/// <seealso cref="ContentControl.Content"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TContentControl WithContent<TContentControl>(this TContentControl @this, object content)
		where TContentControl : ContentControl
	{
		@this.Content = content;
		return @this;
	}
}
