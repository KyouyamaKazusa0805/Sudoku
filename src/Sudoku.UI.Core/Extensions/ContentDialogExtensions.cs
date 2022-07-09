namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with extension methods on <see cref="ContentDialog"/>.
/// </summary>
/// <seealso cref="ContentDialog"/>
public static class ContentDialogExtensions
{
	/// <summary>
	/// Sets the property <see cref="ContentDialog.Title"/> with the specified value.
	/// </summary>
	/// <seealso cref="ContentDialog.Title"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog WithTitle<TNotNull>(this ContentDialog @this, TNotNull title) where TNotNull : notnull
	{
		@this.Title = title;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="ContentControl.Content"/> with the specified value.
	/// </summary>
	/// <seealso cref="ContentControl.Content"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog WithContent(this ContentDialog @this, string content)
	{
		@this.Content = content;
		return @this;
	}
}
