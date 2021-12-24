namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute that applies to a set of pointer-typed parameters, to tell the compiler that
/// those parameters disallow a same pointer value to pass into.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class RestrictAttribute : Attribute
{
}
