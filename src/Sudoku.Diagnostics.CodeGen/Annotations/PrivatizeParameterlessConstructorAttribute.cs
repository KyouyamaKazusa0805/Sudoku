namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Marks onto a <see langword="class"/> type, to tell the source generator the type should contain
/// a <see langword="private"/> parameterless constructor that can't be accessed out of its type field.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PrivatizeParameterlessConstructorAttribute : Attribute
{
}
