namespace Microsoft.UI.Xaml.Documents;

/// <summary>
/// Provides with extension methods on <see cref="Hyperlink"/>.
/// </summary>
/// <seealso cref="Hyperlink"/>
public static class HyperlinkExtensions
{
	/// <summary>
	/// Sets the property <see cref="Hyperlink.UnderlineStyle"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Hyperlink WithUnderlineStyle(this Hyperlink @this, UnderlineStyle underlineStyle)
	{
		@this.UnderlineStyle = underlineStyle;
		return @this;
	}

	/// <summary>
	/// Registers the event <see cref="Hyperlink.Click"/> with the specified delegate typed instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Hyperlink AppendClickEventHandler(this Hyperlink @this, TypedEventHandler<Hyperlink, HyperlinkClickEventArgs> eventHandler)
	{
		@this.Click += eventHandler;
		return @this;
	}
}
