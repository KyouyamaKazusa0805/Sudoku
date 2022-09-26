namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// To mark onto a step searcher, to tell the runtime and the compiler that the type is a step searcher.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class StepSearcherAttribute : Attribute
{
}
