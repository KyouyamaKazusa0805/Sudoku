namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that indicates the default-value generator member, whose name is bound with a dependency property or attached property.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DefaultValueAttribute : Attribute;
