namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute that is marked onto a parameter, to indicate the parameter is a discard
/// and can't be used later in any way.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class IsDiscardAttribute : Attribute
{
}
