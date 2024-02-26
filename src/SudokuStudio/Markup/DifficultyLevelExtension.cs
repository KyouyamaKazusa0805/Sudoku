namespace SudokuStudio.Markup;

/// <summary>
/// Represents difficulty level markup extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(DifficultyLevel))]
[ContentProperty(Name = nameof(Value))]
public sealed class DifficultyLevelExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the value.
	/// </summary>
	public DifficultyLevel Value { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Value;
}
