namespace SudokuStudio.Markup;

/// <summary>
/// Represents a markup extension to provide a <see cref="BabaGroupInitialLetter"/> value.
/// </summary>
/// <seealso cref="BabaGroupInitialLetter"/>
[MarkupExtensionReturnType(ReturnType = typeof(BabaGroupInitialLetter))]
[ContentProperty(Name = nameof(Value))]
public sealed class BabaGroupInitialLetterExtension : MarkupExtension
{
	/// <summary>
	/// The target value to be set.
	/// </summary>
	public BabaGroupInitialLetter Value { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Value;
}
