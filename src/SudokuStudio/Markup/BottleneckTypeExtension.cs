namespace SudokuStudio.Markup;

/// <summary>
/// Represents a markup extension to provide a <see cref="BottleneckType"/> value.
/// </summary>
/// <seealso cref="BottleneckType"/>
[MarkupExtensionReturnType(ReturnType = typeof(BottleneckType))]
[ContentProperty(Name = nameof(Value))]
public sealed class BottleneckTypeExtension : MarkupExtension
{
	/// <summary>
	/// The target value to be set.
	/// </summary>
	public BottleneckType Value { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Value;
}
