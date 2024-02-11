namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that defines an attached property.
/// </summary>
/// <typeparam name="T">Indicates the type of the property evaluated.</typeparam>
/// <param name="propertyName"><inheritdoc/></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class AttachedPropertyAttribute<T>([StringSyntax(StringSyntax.Regex)] string propertyName) : XamlBindingAttribute<T>(propertyName);
