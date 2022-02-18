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
	/// <param name="severity">The severity.</param>
	/// <param name="content">The content to replace with.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	/// <seealso cref="InfoBar.Content"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static InfoBar WithText(this InfoBar @this, InfoBarSeverity severity, string content)
	{
		@this.Severity = severity;
		@this.Content = new TextBlock { Text = content, Padding = new(0, 0, 0, 20) };
		return @this;
	}
}
