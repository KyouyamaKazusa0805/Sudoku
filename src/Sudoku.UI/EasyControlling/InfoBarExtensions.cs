namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides extension methods on <see cref="InfoBar"/>.
/// </summary>
/// <seealso cref="InfoBar"/>
internal static class InfoBarExtensions
{
	/// <summary>
	/// To open the <see cref="InfoBar"/>.
	/// </summary>
	/// <param name="this">The <see cref="InfoBar"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Open(this InfoBar @this) => @this.IsOpen = true;

	/// <summary>
	/// Sets the property <see cref="InfoBar.Content"/> to the target string value.
	/// </summary>
	/// <param name="this">The <see cref="InfoBar"/> instance.</param>
	/// <param name="content">The content to replace with.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	/// <seealso cref="InfoBar.Content"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static InfoBar WithMessage(this InfoBar @this, string content)
	{
		@this.Message = content;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="InfoBar.ActionButton"/> to a <see cref="HyperlinkButton"/>.
	/// </summary>
	/// <param name="this">The <see cref="InfoBar"/> instance.</param>
	/// <param name="link">The link.</param>
	/// <param name="contentDescription">The content description.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	/// <seealso cref="InfoBar.ActionButton"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static InfoBar WithLinkButton(this InfoBar @this, string link, string contentDescription)
	{
		@this.ActionButton = new HyperlinkButton { NavigateUri = new(link), Content = contentDescription };
		return @this;
	}
}
