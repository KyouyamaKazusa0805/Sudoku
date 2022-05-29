namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that determines which data template can be used on a setting item.
/// </summary>
public sealed class SettingItemDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the template that is used for a boolean value.
	/// </summary>
	public DataTemplate BooleanValueTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the default template.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when data template cannot be found.</exception>
	protected override DataTemplate SelectTemplateCore(object item)
	{
		return item switch
		{
			SettingItem { PreferenceValueName: var fieldType } when getFieldType(fieldType) == typeof(bool)
				=> BooleanValueTemplate,
			_ => DefaultTemplate
		};


		static Type? getFieldType(string name) => typeof(UserPreference).GetField(name)?.FieldType;
	}
}
