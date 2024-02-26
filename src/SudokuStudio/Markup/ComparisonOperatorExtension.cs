namespace SudokuStudio.Markup;

/// <summary>
/// Represents comparison operator markup extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(ComparisonOperator))]
[ContentProperty(Name = nameof(Value))]
public sealed class ComparisonOperatorExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the value.
	/// </summary>
	public ComparisonOperator Value { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Value;
}
