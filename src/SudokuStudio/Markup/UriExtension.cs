namespace SudokuStudio.Markup;

/// <summary>
/// Represents a markup extension for creating a <see cref="Uri"/> instance.
/// </summary>
/// <seealso cref="Uri"/>
[MarkupExtensionReturnType(ReturnType = typeof(Uri))]
[ContentProperty(Name = nameof(Text))]
public sealed class UriExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the text to be assigned.
	/// </summary>
	public string Text { get; set; } = string.Empty;


	/// <inheritdoc/>
	protected override object ProvideValue() => new Uri(Text);
}
