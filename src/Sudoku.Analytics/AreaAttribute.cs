namespace Sudoku.Analytics;

/// <summary>
/// Represents an attribute type that is applied to a <see cref="StepSearcher"/>,
/// indicating the enabled area that the step searcher can be invoked.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AreaAttribute : StepSearcherMetadataItemAttribute
{
	/// <summary>
	/// Indicates whether the specified step searcher is allowed in default searching environment.
	/// </summary>
	public required bool AllowDefault { get; init; }

	/// <summary>
	/// Indicates whether the specified step searcher is allowed in gathering environment.
	/// </summary>
	public required bool AllowGathering { get; init; }

	/// <inheritdoc/>
	public override bool IsRequired => false;
}
