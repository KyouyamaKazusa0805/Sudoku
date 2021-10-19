namespace Sudoku.CodeGenerating.GlobalConfigs;

/// <summary>
/// Indicates whether the <see cref="ISourceGenerator"/> or <see cref="IIncrementalGenerator"/> instances
/// will emit the <c>#nullable enable</c> directives into the generated code.
/// </summary>
[AttributeUsage(Class, AllowMultiple = false, Inherited = true)]
public sealed class EmitNullableEnableAttribute : Attribute
{
}
