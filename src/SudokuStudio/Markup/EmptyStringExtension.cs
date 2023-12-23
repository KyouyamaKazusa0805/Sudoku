namespace SudokuStudio.Markup;

/// <summary>
/// A markup extension for empty strings.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class EmptyStringExtension : MarkupExtension
{
	/// <inheritdoc/>
	protected override object ProvideValue() => string.Empty;
}
