namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Defines an attribute type that is applied to a <see cref="StepSearcher"/> derived type,
/// indicating the searching logic only uses cached fields in type <see cref="CachedFields"/>,
/// and it does not use field <see cref="AnalysisContext.Grid"/>.
/// </summary>
/// <seealso cref="CachedFields"/>
/// <seealso cref="AnalysisContext.Grid"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CachedOnlyAttribute : StepSearcherMetadataAttribute;
