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
	/// Sets the property <see cref="ContentDialog.CloseButtonText"/> with the specified value.
	/// </summary>
	/// <seealso cref="ContentDialog.CloseButtonText"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog WithCloseButtonText(this ContentDialog @this, string closeButtonText)
	{
		@this.CloseButtonText = closeButtonText;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="ContentDialog.DefaultButton"/> with the specified value.
	/// </summary>
	/// <seealso cref="ContentDialog.DefaultButton"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog WithDefaultButton(this ContentDialog @this, ContentDialogButton defaultButton)
	{
		@this.DefaultButton = defaultButton;
		return @this;
	}
}
