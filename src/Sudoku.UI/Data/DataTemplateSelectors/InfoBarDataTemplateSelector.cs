namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that selects the <see cref="DataTemplate"/> between
/// <see cref="InfoBarMessage"/> and <see cref="HyperlinkMessage"/> as the model types.
/// </summary>
/// <seealso cref="InfoBarMessage"/>
/// <seealso cref="HyperlinkMessage"/>
public sealed class InfoBarDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the data template that is used by the type <see cref="PlainMessage"/>.
	/// </summary>
	[DataTemplateModelType<PlainMessage>]
	public DataTemplate PlainMessageTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="HyperlinkMessage"/>.
	/// </summary>
	[DataTemplateModelType<HyperlinkMessage>]
	public DataTemplate HyperlinkMessageTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="ManualSolverResultMessage"/>.
	/// </summary>
	[DataTemplateModelType<ManualSolverResultMessage>]
	public DataTemplate AnalysisResultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type of the argument <paramref name="item"/> doesn't derive
	/// from <see cref="InfoBarMessage"/>.
	/// </exception>
	protected override DataTemplate SelectTemplateCore(object item)
	{
		var itemType = item.GetType();
		var query =
			from pi in GetType().GetProperties()
			let types = pi.GetGenericAttributeTypeArguments(typeof(DataTemplateModelTypeAttribute<>))
			where types is [var type] && type == itemType
			select pi;
		return (DataTemplate)query.First().GetValue(this)!;
	}
}
