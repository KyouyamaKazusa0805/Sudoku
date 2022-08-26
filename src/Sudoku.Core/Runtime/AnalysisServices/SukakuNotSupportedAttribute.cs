namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Defines an attribute annotation that can be applied to a step searcher type,
/// indicating the step searcher instance is not supported in sukaku solving mode.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SukakuNotSupportedAttribute : Attribute
{
}
