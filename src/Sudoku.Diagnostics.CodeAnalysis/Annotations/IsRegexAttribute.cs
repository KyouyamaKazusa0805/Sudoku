namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates a field holds a regular expression that represents using a <see cref="string"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class IsRegexAttribute : Attribute
{
}
