namespace Sudoku.UI.DataConversion.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that can determine which data template will be applied to a menu item.
/// </summary>
public sealed class MenuItemDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the basic template.
	/// </summary>
	public DataTemplate BasicTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the basic template without symbol icon.
	/// </summary>
	public DataTemplate BasicTemplateWithoutSymbolIcon { get; set; } = null!;

	/// <summary>
	/// Indicates the separator template.
	/// </summary>
	public DataTemplate SeparatorTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the header template.
	/// </summary>
	public DataTemplate HeaderTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item)
		=> item switch
		{
			MenuItemTemplateData { Symbol: 0, Visibility: Visibility.Collapsed } => BasicTemplateWithoutSymbolIcon,
			MenuItemTemplateData { Visibility: Visibility.Visible } => BasicTemplate,
			MenuItemSeparatorTemplateData => SeparatorTemplate,
			MenuItemHeaderTemplateData => HeaderTemplate,
			_ => throw new InvalidOperationException("The type or value is invalid.")
		};
}
