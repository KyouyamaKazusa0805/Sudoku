namespace Sudoku.Solving.Manual;

/// <summary>
/// Marks onto a <see cref="IStepSearcher"/>, to let the compiler know the specified step searcher
/// is fixed-ordering, and we can't modify the order in the settings UI.
/// </summary>
/// <seealso cref="IStepSearcher"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class IsOptionsFixedAttribute : Attribute
{
}
