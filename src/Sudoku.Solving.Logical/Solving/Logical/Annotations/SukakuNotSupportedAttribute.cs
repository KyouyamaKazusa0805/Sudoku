namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines an attribute annotation that can be applied to a step searcher type,
/// indicating the step searcher instance is not supported in sukaku solving mode.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[Obsolete($"The type is being deprecated. Please use the option '{nameof(StepSearcherRunningOptions.OnlyForStandardSudoku)}' instead.", false)]
public sealed class SukakuNotSupportedAttribute : Attribute;
