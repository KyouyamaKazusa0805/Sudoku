namespace Sudoku.CodeGenerating;

/// <summary>
/// Marks onto a <see langword="class"/> type, to tell the source generator the type should contain
/// a <see langword="private"/> parameterless constructor that can't be accessed out of its type field.
/// </summary>
[AttributeUsage(Class, AllowMultiple = false, Inherited = false)]
public sealed class PrivatizeParameterlessConstructorAttribute : Attribute
{
}
