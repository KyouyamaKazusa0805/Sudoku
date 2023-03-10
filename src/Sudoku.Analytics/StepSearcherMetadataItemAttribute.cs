namespace Sudoku.Analytics;

/// <summary>
/// Represents an attribute type that is applied to a <see cref="StepSearcher"/> instance.
/// </summary>
public abstract class StepSearcherMetadataItemAttribute : Attribute
{
	/// <summary>
	/// Indicates whether the metadata item is required.
	/// </summary>
	public abstract bool IsRequired { get; }
}
