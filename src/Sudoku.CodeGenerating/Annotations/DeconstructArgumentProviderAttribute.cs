namespace Sudoku.CodeGenerating;

/// <summary>
/// To mark onto a method to tell the source generator that the method is an argument provider
/// that is used for extension deconstruction methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class DeconstructArgumentProviderAttribute : Attribute
{
}
