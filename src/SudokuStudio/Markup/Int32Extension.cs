namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that can parse a <see cref="string"/> value, converting it into a valid <see cref="int"/>
/// or throwing exceptions when the code is invalid.
/// </summary>
[ContentProperty(Name = nameof(Value))]
[MarkupExtensionReturnType(ReturnType = typeof(int))]
public sealed class Int32Extension : MarkupExtension
{
	/// <summary>
	/// Indicates the target value input.
	/// </summary>
	public int Value { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Value;
}
