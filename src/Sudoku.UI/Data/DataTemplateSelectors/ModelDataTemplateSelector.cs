namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Extracts a type that is used by derived types, providing them with a template way to select data template
/// automatically and binding with a model type.
/// </summary>
public abstract class ModelDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the default template. The value can be <see langword="null"/> if you don't want to provide
	/// with such data template as the default case.
	/// </summary>
	[DisallowNull]
	public DataTemplate? DefaultTemplate { get; set; }


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">
	/// Throws when the default template property <see cref="DefaultTemplate"/> is <see langword="null"/> currently,
	/// but the argument <paramref name="item"/> is <see langword="null"/>.
	/// </exception>
	protected sealed override DataTemplate SelectTemplateCore(object? item)
	{
		if (item is null)
		{
			return DefaultTemplate ?? throw new InvalidOperationException("Default data template cannot be found.");
		}

		var itemType = item.GetType();
		var query =
			from pi in GetType().GetProperties()
			let types = pi.GetGenericAttributeTypeArguments(typeof(DataTemplateModelTypeAttribute<>))
			where types is [var type] && type == itemType
			select pi;
		return (DataTemplate?)query.FirstOrDefault()?.GetValue(this)
			?? DefaultTemplate
			?? throw new InvalidOperationException("Default data template cannot be found.");
	}
}
