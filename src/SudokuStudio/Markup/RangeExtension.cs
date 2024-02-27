namespace SudokuStudio.Markup;

/// <summary>
/// Represents range extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(Range))]
[ContentProperty(Name = nameof(Range))]
public sealed class RangeExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the range value.
	/// </summary>
	public string Range { get; set; } = null!;


	/// <inheritdoc/>
	protected override object ProvideValue()
		=> Range.Split("..") is [var left, var right] && int.TryParse(left, out var l) && int.TryParse(right, out var r)
			? l..r
			: ..;
}
