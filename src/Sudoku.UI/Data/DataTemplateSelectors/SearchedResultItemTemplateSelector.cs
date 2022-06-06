namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a selector that can select the data template on searched result items.
/// </summary>
public sealed class SearchedResultItemTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the template that is used by the valid seared result.
	/// </summary>
	[DataTemplateModelType<SearchedResult>]
	public DataTemplate PairInfoTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the default template.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item)
	{
		if (item is null)
		{
			return DefaultTemplate;
		}

		var itemType = item.GetType();
		var query =
			from pi in GetType().GetProperties()
			let types = pi.GetGenericAttributeTypeArguments(typeof(DataTemplateModelTypeAttribute<>))
			where types is [var type] && type == itemType
			select pi;
		return (DataTemplate)query.First().GetValue(this)!;
	}
}
