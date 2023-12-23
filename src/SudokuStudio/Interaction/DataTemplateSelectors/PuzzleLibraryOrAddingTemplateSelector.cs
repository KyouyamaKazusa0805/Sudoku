namespace SudokuStudio.Interaction.DataTemplateSelectors;

/// <summary>
/// Represents a template selector for puzzle library displaying or adding operation.
/// </summary>
public sealed class PuzzleLibraryOrAddingTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// The basic display template.
	/// </summary>
	public DataTemplate DisplayTemplate { get; set; } = null!;

	/// <summary>
	/// The adding operation template.
	/// </summary>
	public DataTemplate AddingOperationTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item)
		=> item switch
		{
			PuzzleLibraryBindableSource { IsAddingOperationPlaceholder: var isPlaceholder }
				=> isPlaceholder ? AddingOperationTemplate : DisplayTemplate,
			_ => throw new InvalidOperationException($"The argument must be of type '{nameof(PuzzleLibraryBindableSource)}'.")
		};
}
