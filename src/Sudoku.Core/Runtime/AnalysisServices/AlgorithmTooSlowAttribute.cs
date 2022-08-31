namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Defines an attribute annotation that can be applied to a step searcher type,
/// indicating an algorithm chosen for a step searcher is too slow.
/// </summary>
/// <remarks>
/// This attribute can be ignored when a manual solver enables them regardless of execution speed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AlgorithmTooSlowAttribute : Attribute
{
}
