namespace SudokuStudio.Interaction.StyleSelectors;

/// <summary>
/// Represents a style selector for puzzle library operation.
/// </summary>
public sealed class PuzzleLibraryOperationStyleSelector : StyleSelector
{
	/// <summary>
	/// Indicates the normal style.
	/// </summary>
	public Style NormalStyle { get; set; } = null!;

	/// <summary>
	/// Indicates the adding style.
	/// </summary>
	public Style AddingStyle { get; set; } = null!;


	/// <inheritdoc/>
	protected override Style SelectStyleCore(object item, DependencyObject container)
		=> item switch
		{
			PuzzleLibraryBindableSource { IsAddingOperationPlaceholder: var isPlaceholder } => isPlaceholder ? AddingStyle : NormalStyle,
			_ => throw new InvalidOperationException($"The argument must be of type {nameof(InvalidOperationException)}.")
		};
}
