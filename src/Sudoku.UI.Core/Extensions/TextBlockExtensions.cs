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

	/// <summary>
	/// Sets the property <see cref="TextBlock.Text"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithText(this TextBlock @this, string text)
	{
		@this.Text = text;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextBlock.Text"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithText<T>(this TextBlock @this, T textObject)
		=> textObject?.ToString() is { } text ? @this.WithText(text) : @this;

	/// <summary>
	/// Sets the property <see cref="TextBlock.FontFamily"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithFontFamily(this TextBlock @this, string fontFamily)
		=> @this.WithFontFamily(new FontFamily(fontFamily));

	/// <summary>
	/// Sets the property <see cref="TextBlock.FontFamily"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithFontFamily(this TextBlock @this, FontFamily fontFamily)
	{
		@this.FontFamily = fontFamily;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextBlock.FontSize"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithFontSize(this TextBlock @this, double fontSize)
	{
		@this.FontSize = fontSize;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextBlock.FontWeight"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithFontWeight(this TextBlock @this, FontWeight fontWeight)
	{
		@this.FontWeight = fontWeight;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextBlock.FontStyle"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithFontStyle(this TextBlock @this, FontStyle fontStyle)
	{
		@this.FontStyle = fontStyle;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextBlock.HorizontalTextAlignment"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithHorizontalTextAlignment(this TextBlock @this, TextAlignment textAlignment)
	{
		@this.HorizontalTextAlignment = textAlignment;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="TextBlock.TextAlignment"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock WithTextAlignment(this TextBlock @this, TextAlignment textAlignment)
	{
		@this.TextAlignment = textAlignment;
		return @this;
	}
}
