namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that indicates the callback method of the bound dependency property or attached property.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class CallbackAttribute : Attribute;
