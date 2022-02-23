namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that selects the <see cref="DataTemplate"/> between
/// <see cref="InfoBarInfo"/> and <see cref="InfoBarInfoWithLink"/> as the model types.
/// </summary>
/// <seealso cref="InfoBarInfo"/>
/// <seealso cref="InfoBarInfoWithLink"/>
public sealed class InfoBarTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the data template that is used by the type <see cref="InfoBarInfo"/>.
	/// </summary>
	public DataTemplate InfoBarInfoTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="InfoBarInfoWithLink"/>.
	/// </summary>
	public DataTemplate InfoBarInfoWithLinkTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item) =>
		item switch
		{
			InfoBarInfo => InfoBarInfoTemplate,
			InfoBarInfoWithLink => InfoBarInfoWithLinkTemplate
		};
}
