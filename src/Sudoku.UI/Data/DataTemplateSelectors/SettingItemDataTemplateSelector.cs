namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that determines which data template can be used on a setting item.
/// </summary>
public sealed class SettingItemDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the template that is used for a toggle switch.
	/// </summary>
	[DataTemplateModelTypeAttribute<ToggleSwitchSettingItem>]
	public DataTemplate ToggleSwitchTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the template that is used for a slider.
	/// </summary>
	[DataTemplateModelTypeAttribute<SliderSettingItem>]
	public DataTemplate SliderTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the default template.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object? item)
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
