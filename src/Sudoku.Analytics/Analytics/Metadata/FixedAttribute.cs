namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Indicates whether the option is fixed that can't be modified in UI.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class FixedAttribute : StepSearcherMetadataAttribute;
