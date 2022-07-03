namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines the type of the field that will route.
/// </summary>
/// <typeparam name="T">The routed type.</typeparam>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class TypeAttribute<T> : Attribute
{
}
