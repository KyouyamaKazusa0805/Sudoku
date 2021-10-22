namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates the method is provided as a lambda conversion that is used by source generators.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class LambdaBodyAttribute : Attribute
{
}
