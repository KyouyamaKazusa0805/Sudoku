namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that applies to a data template property.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DataTemplateModelTypeAttribute<TModel> : Attribute
{
}
