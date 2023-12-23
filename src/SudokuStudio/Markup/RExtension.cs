namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that only fetches for string resources defined by folder <c>Resources</c>.
/// </summary>
[ContentProperty(Name = nameof(Key))]
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class RExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the key of the resource.
	/// </summary>
	public string Key { get; set; } = string.Empty;


	/// <inheritdoc/>
	protected override object ProvideValue() => GetString(Key);
}
