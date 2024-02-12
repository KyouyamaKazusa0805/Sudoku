namespace SudokuStudio.Interaction.DataTemplateSelectors;

/// <summary>
/// Represents a data template selector for library tags.
/// </summary>
public sealed class LibraryTagsDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the template used when values are available.
	/// </summary>
	public DataTemplate ValuesTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the template used when no values are available.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object? item) => item switch { string[] => ValuesTemplate, _ => DefaultTemplate };
}
