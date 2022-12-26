namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines an attribute annotation that can be applied to a step searcher type,
/// indicating an algorithm chosen for a step searcher is too slow.
/// </summary>
/// <remarks>
/// This attribute can be ignored when a logical solver enables them regardless of execution speed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[Obsolete($"The type is being deprecated. Please use option '{nameof(StepSearcherRunningOptions.SlowAlgorithm)}' instead.", false)]
public sealed class AlgorithmTooSlowAttribute : Attribute
{
}
