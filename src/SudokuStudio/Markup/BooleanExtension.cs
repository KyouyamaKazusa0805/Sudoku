namespace SudokuStudio.Markup;

/// <summary>
/// Represents a markup extension to provide a <see cref="bool"/> value.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(bool))]
[ContentProperty(Name = nameof(Value))]
public sealed class BooleanExtension : MarkupExtension
{
	/// <summary>
	/// The target value to be set.
	/// </summary>
	public bool Value { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Value;
}
