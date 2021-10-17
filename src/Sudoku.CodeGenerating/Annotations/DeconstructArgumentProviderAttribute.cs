namespace Sudoku.CodeGenerating;

/// <summary>
/// To mark onto a method to tell the source generator that the method is an argument provider
/// that is used for extension deconstruction methods.
/// </summary>
[AttributeUsage(Method, AllowMultiple = false, Inherited = false)]
public sealed class DeconstructArgumentProviderAttribute : Attribute
{
}
