namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Indicates the type is a runnable <see cref="StepSearcher"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StepSearcherAttribute : StepSearcherMetadataAttribute;
