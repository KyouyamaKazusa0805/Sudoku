namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute, to apply to a type parameter, which means the type parameter should satisfy
/// CRTP (Curiously Recursive Template Pattern) in C#.
/// </summary>
[AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
public sealed class SelfAttribute : Attribute
{
}
