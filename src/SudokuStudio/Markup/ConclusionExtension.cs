namespace SudokuStudio.Markup;

/// <summary>
/// Represents a markup extension used by creating <see cref="Conclusion"/> instances.
/// </summary>
/// <seealso cref="Conclusion"/>
public sealed class ConclusionExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the conclusion string.
	/// </summary>
	public string Text { get; set; } = null!;


	/// <inheritdoc/>
	protected override object ProvideValue() => Conclusion.Parse(Text);
}
