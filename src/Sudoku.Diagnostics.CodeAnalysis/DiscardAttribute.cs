namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute that is used onto a parameter, to indicate the parameter
/// is a discard, which means it can't be used anyway.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class DiscardAttribute : Attribute
{
}
