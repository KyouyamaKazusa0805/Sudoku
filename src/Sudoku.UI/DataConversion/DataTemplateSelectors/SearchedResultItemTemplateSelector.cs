namespace Sudoku.UI.DataConversion.DataTemplateSelectors;

/// <summary>
/// Defines a selector that can select the data template on searched result items.
/// </summary>
public sealed class SearchedResultItemTemplateSelector : ModelDataTemplateSelector
{
	/// <summary>
	/// Indicates the template that is used by the valid seared result.
	/// </summary>
	[DataTemplateModelType<SearchedResult>]
	public DataTemplate PairInfoTemplate { get; set; } = null!;
}
