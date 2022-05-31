namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a selector that can select the data template on searched result items.
/// </summary>
public sealed class SearchedResultItemTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the template that is used by the valid seared result.
	/// </summary>
	public DataTemplate PairInfoTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the default template.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item)
		=> item switch
		{
			SearchedResult => PairInfoTemplate,
			_ => DefaultTemplate
		};
}
