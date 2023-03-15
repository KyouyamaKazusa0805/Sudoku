namespace Sudoku.Analytics.Metadata;

/// <summary>
/// <para>Indicates whether the option is fixed that can't be modified in UI.</para>
/// <para><i>The default value is <see langword="false"/>.</i></para>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class FixedAttribute : StepSearcherMetadataAttribute;
