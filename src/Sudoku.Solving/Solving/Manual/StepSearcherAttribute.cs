namespace Sudoku.Solving.Manual;

/// <summary>
/// To mark onto a step searcher, to tell the runtime and the compiler that the type is a step searcher.
/// </summary>
/// <seealso cref="IStepSearcher"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class StepSearcherAttribute : Attribute
{
}
