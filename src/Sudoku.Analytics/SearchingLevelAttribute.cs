namespace Sudoku.Analytics;

/// <summary>
/// Represents an attribute type that is applied to a <see cref="StepSearcher"/>,
/// indicating the searching level of the step searcher to be invoked.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SearchingLevelAttribute : StepSearcherMetadataItemAttribute
{
	/// <summary>
	/// Initializes a <see cref="SearchingLevelAttribute"/> instane via the level.
	/// </summary>
	/// <param name="level">The level.</param>
	public SearchingLevelAttribute(int level) => Level = level;


	/// <summary>
	/// Indicates the level. The greater the value will be, the lower level the step searcher will be invoked.
	/// </summary>
	public int Level { get; }

	/// <inheritdoc/>
	public override bool IsRequired => true;
}
