namespace Microsoft.UI.Xaml.Documents;

/// <summary>
/// Provides with extension methods on <see cref="Span"/>.
/// </summary>
/// <seealso cref="Span"/>
public static class SpanExtensions
{
	/// <summary>
	/// Append inline into the property <see cref="Span.Inlines"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TSpan AppendInline<TSpan>(this TSpan @this, Inline inline) where TSpan : Span
	{
		@this.Inlines.Add(inline);
		return @this;
	}
}
