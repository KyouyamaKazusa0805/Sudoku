namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that selects the <see cref="DataTemplate"/> between
/// <see cref="InfoBarInfo"/> and <see cref="InfoBarInfoWithLink"/> as the model types.
/// </summary>
/// <seealso cref="InfoBarInfo"/>
/// <seealso cref="InfoBarInfoWithLink"/>
public sealed class InfoBarDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the data template that is used by the type <see cref="InfoBarInfo"/>.
	/// </summary>
	public DataTemplate PlainInfoBarInfoTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="InfoBarInfoWithLink"/>.
	/// </summary>
	public DataTemplate InfoBarInfoWithLinkTemplate { get; set; } = null!;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type of the argument <paramref name="item"/> doesn't derive
	/// from <see cref="InfoBarInfo"/>.
	/// </exception>
	protected override DataTemplate SelectTemplateCore(object item) =>
		item switch
		{
			PlainInfoBarInfo => PlainInfoBarInfoTemplate,
			InfoBarInfoWithLink => InfoBarInfoWithLinkTemplate,
			_ => throw new InvalidOperationException("The type is invalid.")
		};
}
