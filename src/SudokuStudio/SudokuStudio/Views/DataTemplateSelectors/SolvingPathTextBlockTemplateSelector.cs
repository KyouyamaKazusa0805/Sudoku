namespace SudokuStudio.Views.DataTemplateSelectors;

/// <summary>
/// Defines a template selector that is applied to a solving path element.
/// </summary>
public sealed class SolvingPathTextBlockTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the default template.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = new();

	/// <summary>
	/// Indicates the step template.
	/// </summary>
	public DataTemplate? StepTemplate { get; set; }


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item) => item switch { IStep => StepTemplate, _ => DefaultTemplate } ?? DefaultTemplate;
}
