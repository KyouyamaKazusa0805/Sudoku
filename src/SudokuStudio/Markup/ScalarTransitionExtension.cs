namespace SudokuStudio.Markup;

/// <summary>
/// Represents a scalar transition extension instance.
/// </summary>
public sealed class ScalarTransitionExtension : MarkupExtension
{
	/// <inheritdoc cref="ScalarTransition.Duration"/>
	public TimeSpan? Duration { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => Duration is { } duration ? new ScalarTransition { Duration = duration } : new ScalarTransition();
}
