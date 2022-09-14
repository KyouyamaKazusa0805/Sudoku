namespace Microsoft.UI.Xaml.Documents;

/// <summary>
/// Provides with extension methods on <see cref="TextElement"/>.
/// </summary>
/// <seealso cref="TextElement"/>
public static class TextElementExtensions
{
	/// <summary>
	/// Sets the property <see cref="TextElement.Foreground"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTextElement WithForeground<TTextElement>(this TTextElement @this, Brush brush)
		where TTextElement : TextElement
	{
		@this.Foreground = brush;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextElement.Foreground"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTextElement WithForegroundIfNotNull<TTextElement>(this TTextElement @this, Brush? brush)
		where TTextElement : TextElement
	{
		if (brush is not null)
		{
			@this.Foreground = brush;
		}

		return @this;
	}
}
